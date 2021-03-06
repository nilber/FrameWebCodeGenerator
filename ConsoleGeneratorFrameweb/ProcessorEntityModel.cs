﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeradorFrameweb
{
    public class ProcessorEntityModel : Processor
    {


        public ProcessorEntityModel(Config _config) : base(_config)
        {
        }


        public override void Execute(Component componente)
        {


            /// DomainPackage 
            /// 


            var package_domains = componente.Components.Where(y => y.xsi_type == "frameweb:DomainPackage").ToList();
            foreach (var package_domain in package_domains)
            {
                var dir_output_class_package = this.BuildDirectoryStructures(Config.dir_output_class, package_domain.name);


                var domainClass = package_domain.Components.Where(y => y.xsi_type == "frameweb:DomainClass").ToList();

                foreach (var _class in domainClass)
                {
                    Component generalization = null;
                    var tags_class = new Dictionary<string, string>();
                    tags_class.Add("FW_CLASS_NAME", _class.name);

                    if (_class.Components != null)
                    {
                        generalization = _class.Components.Where(x => x.tag == "generalization").FirstOrDefault();

                        if (_class.Components.Any(x => x.xsi_type == "frameweb:DomainMethod" && x.isAbstract))
                            tags_class.Add("FW_CLASS_VISIBILITY", "public abstract");
                        else
                            tags_class.Add("FW_CLASS_VISIBILITY", "public");

                        if (generalization != null)
                        {
                            var _generalization = generalization.generalizationSet.Split('/');
                            var _str_generalization = _generalization[_generalization.Length - 1];
                            if (_str_generalization.Contains('.'))
                            {
                                _str_generalization = _str_generalization.Split('.')[0];
                            }
                            tags_class.Add("FW_EXTENDS", "extends " + _str_generalization);
                        }
                        else
                        {
                            tags_class.Add("FW_EXTENDS", string.Empty);
                        }


                        var _class_propeties = _class.Components.Where(x => (new List<string> { "frameweb:DomainAttribute", "frameweb:IdAttribute", "frameweb:DateTimeAttribute" }).Contains(x.xsi_type)).ToList();

                        string properties = string.Empty;
                        foreach (var propertie in _class_propeties)
                        {
                            var text_parameter = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + propertie.getXsiTypeFile());
                            text_parameter = text_parameter.Replace("FW_PARAMETER_TYPE", propertie.GetTypeDomainAttribute());
                            text_parameter = text_parameter.Replace("FW_PARAMETER_FIRST_UPPER", Utilities.FirstCharToUpper(propertie.name));
                            text_parameter = text_parameter.Replace("FW_PARAMETER", propertie.name);
                            text_parameter = text_parameter.Replace("FW_VISIBILITY", propertie.visibility);

                            properties += text_parameter;
                        }

                        tags_class.Add("FW_CLASS_PARAMETERS", properties);

                        var class_methods = _class.Components.Where(x => x.xsi_type == "frameweb:DomainMethod").ToList();

                        string methods = string.Empty;
                        foreach (var method in class_methods)
                        {
                            string text_method;
                            if (method.isAbstract)
                            {
                                text_method = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + "Abstract" + method.getXsiTypeFile());
                                text_method = text_method.Replace("FW_METHOD_VISIBILITY", "public abstract");
                            }
                            else
                            {
                                text_method = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + method.getXsiTypeFile());
                                text_method = text_method.Replace("FW_METHOD_VISIBILITY", "public");
                            }


                            text_method = text_method.Replace("FW_METHOD_RETURN_TYPE", method.GetMethodTypeDomainAttribute());
                            text_method = text_method.Replace("FW_METHOD_NAME", method.name);

                            methods += text_method;
                        }

                        tags_class.Add("FW_CLASS_METHOD", methods);

                    }

                    var text = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + _class.getXsiTypeFile());
                    foreach (var item in tags_class)
                    {
                        text = text.Replace(item.Key, item.Value);
                    }

                    File.WriteAllText(Path.Combine(dir_output_class_package, _class.name + Config.ext_class), text);
                }
            }
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeradorFrameweb
{
    public class ProcessorApplicationModel : Processor
    {
        public ProcessorApplicationModel(Config _config) : base(_config)
        {
        }


        public override void Execute(Component componente)
        {

            /// ApplicationPackage
            /// 

            var package_applications = componente.Components.Where(y => y.xsi_type == "frameweb:ApplicationPackage").ToList();
            foreach (var package_application in package_applications)
            {
                var dir_output_class_package = this.BuildDirectoryStructures(Config.dir_output_class, package_application.name);

                var domainClass = package_application.Components.Where(y => y.xsi_type == "frameweb:ServiceClass").ToList();
                foreach (var _class in domainClass)
                {
                    Component generalization = null;
                    var tags_class = new Dictionary<string, string>();
                    tags_class.Add("FW_CLASS_NAME", _class.name);

                    if (_class.Components != null)
                    {
                        generalization = _class.Components.Where(x => x.tag == "generalization").FirstOrDefault();

                        if (_class.Components.Any(x => x.xsi_type == "frameweb:ServiceMethod" && x.isAbstract))
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


                        var _class_propeties = _class.Components.Where(x => x.xsi_type == "frameweb:ServiceAttribute").ToList();

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

                        var class_methods = _class.Components.Where(x => x.xsi_type == "frameweb:ServiceMethod").ToList();

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

                            text_method = text_method.Replace("FW_METHOD_RETURN_TYPE", (string.IsNullOrWhiteSpace(method.methodType)) ? "void" : method.methodType);
                            text_method = text_method.Replace("FW_METHOD_NAME", method.name);

                            if (!string.IsNullOrWhiteSpace(method.methodType))
                                text_method = text_method.Replace("FW_METHOD_RETURN", "return null;");
                            else
                                text_method = text_method.Replace("FW_METHOD_RETURN", string.Empty);


                            text_method = text_method.Replace("FW_METHOD_PARAM", method.GetMethodParameter());


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













                // INTERFACE


                var interfaces = package_application.Components.Where(y => y.xsi_type == "frameweb:ServiceInterface").ToList();

                foreach (var _interface in interfaces)
                {
                    var tags_interface = new Dictionary<string, string>();
                    tags_interface.Add("FW_INTERFACE_NAME", _interface.name);

                    var realizations = componente.Components.SelectMany(x => x.Components).Where(y => y.xsi_type == "frameweb:ServiceRealization" && y.supplier.EndsWith(_interface.name)).ToList();


                    foreach (var realization in realizations)
                    {
                        string methods = string.Empty;
                        var list_class_realization = domainClass.Where(x => x.name == realization.getClient()).ToList();
                        foreach (var class_realization in list_class_realization)
                        {
                            var class_methods = class_realization.Components.Where(x => x.xsi_type == "frameweb:ServiceMethod").ToList();


                            foreach (var method in class_methods)
                            {
                                string text_method = "FW_METHOD_RETURN_TYPE FW_METHOD_NAME(FW_METHOD_PARAMETERS);\n";

                                var methodParameters = method.Components.Where(x => x.tag == "ownedParameter").ToList();
                                string text_method_parameters = string.Empty;
                                if (methodParameters != null && methodParameters.Count() > 0)
                                {
                                    foreach (var methodParameter in methodParameters)
                                    {
                                        text_method_parameters += string.Format("{0} {1},", methodParameter.getType(), methodParameter.name);
                                    }

                                    text_method_parameters = text_method_parameters.Substring(0, text_method_parameters.Length - 1);
                                }

                                text_method = text_method.Replace("FW_METHOD_PARAMETERS", text_method_parameters);
                                text_method = text_method.Replace("FW_METHOD_RETURN_TYPE", method.GetMethodTypeDomainAttribute());
                                text_method = text_method.Replace("FW_METHOD_NAME", method.name);

                                methods += text_method;
                            }
                        }
                        tags_interface.Add("FW_INTERFACE_METHOD", methods);
                    }

                    var text = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + _interface.getXsiTypeFile());
                    foreach (var item in tags_interface)
                    {
                        text = text.Replace(item.Key, item.Value);
                    }

                    File.WriteAllText(Path.Combine(dir_output_class_package, _interface.name + Config.ext_class), text);

                    realizations = null;

                    //
                }
            }

        }

    }
}

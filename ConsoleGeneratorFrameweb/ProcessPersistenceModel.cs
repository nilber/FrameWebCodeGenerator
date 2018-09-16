using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeradorFrameweb
{
    public class ProcessPersistenceModel : Process
    {


        public ProcessPersistenceModel(Config _config) : base(_config)
        {
        }


        public override void Execute(Component componente)
        {
            this.CreateInterfaces(componente);
            this.CreateClasses(componente);
        }

        private void CreateClasses(Component componente)
        {

            //PersistencePackage

            var package_persistences= componente.Components.Where(y => y.xsi_type == "frameweb:PersistencePackage").ToList();
            foreach (var package_persistence in package_persistences)
            {
                var dir_output_class_package = this.BuildDirectoryStructures(config.dir_output_class, package_persistence.name);


                var daoClasses = package_persistence.Components.Where(y => y.xsi_type == "frameweb:DAOClass").ToList();


                foreach (var _class in daoClasses)
                {
                    var tags_class = new Dictionary<string, string>();
                    tags_class.Add("FW_CLASS_NAME", _class.name);


                    Component generalization = _class.Components.Where(x => x.tag == "generalization").FirstOrDefault();

                    if (_class.isAbstract)
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

                    Component realization = componente.Components.SelectMany(x => x.Components).Where(y => y.xsi_type == "frameweb:DAORealization" && y.getClient() == _class.name).FirstOrDefault();


                    if (realization != null)
                    {
                        tags_class.Add("FW_IMPLEMENTS", "implements " + realization.getSupplier());

                        Component daoInterface = componente.Components.SelectMany(x => x.Components).Where(y => y.xsi_type == "frameweb:DAOInterface" && y.name == realization.getSupplier()).FirstOrDefault();

                        if (daoInterface != null)
                        {
                            tags_class.Add("FW_CLASS_IMPLEMENTS_INFIX", "implements " + realization.getSupplier());
                        }
                        else
                        {
                            tags_class.Add("FW_CLASS_IMPLEMENTS_INFIX", "set infix on interface");
                        }
                    }
                    else
                    {
                        tags_class.Add("FW_IMPLEMENTS", string.Empty);
                    }


                    if (_class.Components != null)
                    {

                        var _class_propeties = _class.Components.Where(x => x.xsi_type == "frameweb:DAOAttribute").ToList();

                        string properties = string.Empty;
                        foreach (var propertie in _class_propeties)
                        {
                            var text_parameter = File.ReadAllText(config.dir_template + config.lang + Path.DirectorySeparatorChar + propertie.getXsiTypeFile());
                            text_parameter = text_parameter.Replace("FW_PARAMETER_TYPE", propertie.GetTypeDomainAttribute());
                            text_parameter = text_parameter.Replace("FW_PARAMETER_FIRST_UPPER", Utilities.FirstCharToUpper(propertie.name));
                            text_parameter = text_parameter.Replace("FW_PARAMETER", propertie.name);
                            text_parameter = text_parameter.Replace("FW_VISIBILITY", propertie.visibility);


                            properties += text_parameter;
                        }

                        tags_class.Add("FW_CLASS_PARAMETERS", properties);

                        var class_methods = _class.Components.Where(x => x.xsi_type == "frameweb:DAOMethod").ToList();

                        string methods = string.Empty;
                        foreach (var method in class_methods)
                        {
                            string text_method;
                            if (method.isAbstract)
                            {
                                text_method = File.ReadAllText(config.dir_template + config.lang + Path.DirectorySeparatorChar + "Abstract" + method.getXsiTypeFile());
                                text_method = text_method.Replace("FW_METHOD_VISIBILITY", "public abstract");
                            }
                            else
                            {
                                text_method = File.ReadAllText(config.dir_template + config.lang + Path.DirectorySeparatorChar + method.getXsiTypeFile());
                                text_method = text_method.Replace("FW_METHOD_VISIBILITY", "public");
                            }


                            //text_method = text_method.Replace("FW_METHOD_RETURN_TYPE", method.GetMethodTypeDomainAttribute());
                            //text_method = text_method.Replace("FW_METHOD_NAME", method.name);

                            text_method = text_method.Replace("FW_METHOD_RETURN_TYPE", (string.IsNullOrWhiteSpace(method.GetMethodTypeDomainAttribute())) ? "void" : method.GetMethodTypeDomainAttribute());
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

                    var text = File.ReadAllText(config.dir_template + config.lang + Path.DirectorySeparatorChar + _class.getXsiTypeFile());
                    foreach (var item in tags_class)
                    {
                        text = text.Replace(item.Key, item.Value);
                    }

                    File.WriteAllText(Path.Combine(dir_output_class_package, _class.name + config.ext_class), text);

                }
            }
        }

        private void CreateInterfaces(Component componente)
        {

            var package_persistences = componente.Components.Where(y => y.xsi_type == "frameweb:PersistencePackage").ToList();
            foreach (var package_persistence in package_persistences)
            {
                var dir_output_class_package = this.BuildDirectoryStructures(config.dir_output_class, package_persistence.name);


                var daoInterfaces = package_persistence.Components.Where(y => y.xsi_type == "frameweb:DAOInterface").ToList();

                if (daoInterfaces != null && daoInterfaces.Count > 0)
                {
                    // Copiar arquivos padrões
                }

                foreach (var _interface in daoInterfaces)
                {
                    var tags_class = new Dictionary<string, string>();



                    var daoRealizations = componente.Components.SelectMany(x => x.Components).Where(y => y.xsi_type == "frameweb:DAORealization" && y.getSupplier() == _interface.name).ToList();

                    foreach (var realization in daoRealizations)
                    {
                        tags_class = new Dictionary<string, string>();
                        tags_class.Add("FW_INTERFACE_NAME", _interface.name);
                        tags_class.Add("FW_INTERFACE_INFIX", _interface.infix);


                        // Gerando arquivos

                        var textInterface = File.ReadAllText(config.dir_template + config.lang + Path.DirectorySeparatorChar + _interface.getXsiTypeFile());
                        foreach (var item in tags_class)
                        {
                            textInterface = textInterface.Replace(item.Key, item.Value);
                        }

                        File.WriteAllText(Path.Combine(dir_output_class_package, _interface.name + config.ext_class), textInterface);

                    }

                }
            }
        }

    }
}

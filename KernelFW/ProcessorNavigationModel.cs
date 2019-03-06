using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeradorFrameweb
{
    public class ProcessorNavigationModel : Processor
    {
        public ProcessorNavigationModel(Config _config) : base(_config)
        {
        }

        public override void Execute(Component componente)
        {

            /// CONTROLLER // FrontControllerClass
            var package_controllers = componente.Components.Where(x => x.xsi_type == "frameweb:ControllerPackage").ToList();

            foreach (var package_controller in package_controllers)
            {
                var dir_output_class_package = this.BuildDirectoryStructures(Config.dir_output_class, package_controller.name);
                var class_controllers = package_controller.Components.Where(x => x.xsi_type == "frameweb:FrontControllerClass").ToList();

                foreach (var controller in class_controllers)
                {
                    if (controller.getXsiTypeFile() == "ServiceControllerAssociation.txt" || controller.getXsiTypeFile() == "NavigationGeneralizationSet.txt") continue;


                    var tags_controller = new Dictionary<string, string>();



               //     var generalization = class_controllers.SelectMany(x => x.Components).Where(y => y.tag == "generalization").ToList();



                    Component generalization = controller.Components.Where(x => x.tag == "generalization").FirstOrDefault();
     
                    if (generalization != null)
                    {
                        var _generalization = generalization.generalizationSet.Split('/');
                        var _str_generalization = _generalization[_generalization.Length - 1];
                        if (_str_generalization.Contains('.'))
                        {
                            _str_generalization = _str_generalization.Split('.')[0];
                        }
                        tags_controller.Add("FW_EXTENDS", "extends " + _str_generalization);
                    }
                    else
                    {
                        tags_controller.Add("FW_EXTENDS", string.Empty);
                    }

                    Component realization = componente.Components.SelectMany(x => x.Components).Where(y => y.xsi_type == "frameweb:NavigationRealization" && y.getClient() == controller.name).FirstOrDefault();


                    if (realization != null)
                    {
                        tags_controller.Add("FW_IMPLEMENTS", "implements " + realization.getSupplier());

                        Component daoInterface = componente.Components.SelectMany(x => x.Components).Where(y => y.xsi_type == "frameweb:NavigationInterface" && y.name == realization.getSupplier()).FirstOrDefault();

                        if (daoInterface != null)
                        {
                            tags_controller.Add("FW_CLASS_IMPLEMENTS_INFIX", "implements " + realization.getSupplier());
                        }
                        else
                        {
                            tags_controller.Add("FW_CLASS_IMPLEMENTS_INFIX", "set infix on interface");
                        }
                    }
                    else
                    {
                        tags_controller.Add("FW_IMPLEMENTS", string.Empty);
                    }


                    tags_controller.Add("FW_CLASS_NAME", controller.name);

                    if (controller.isAbstract)
                        tags_controller.Add("FW_CLASS_VISIBILITY", "public abstract");
                    else
                        tags_controller.Add("FW_CLASS_VISIBILITY", "public");


                    var frontControllerDependency = componente.Components.Where(x => x.xsi_type == "frameweb:FrontControllerDependency" && x.getSupplier() == controller.name).FirstOrDefault();
                    tags_controller.Add("FW_BEAN_NAME", frontControllerDependency != null ? frontControllerDependency.getClient() : string.Empty);


                    var controller_parameters = controller.Components.Where(x => x.xsi_type == "frameweb:IOParameter").ToList();

                    string parameters = string.Empty;
                    foreach (var parameter in controller_parameters)
                    {
                        var text_parameter = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + parameter.getXsiTypeFile());
                        text_parameter = text_parameter.Replace("FW_PARAMETER_TYPE", string.IsNullOrWhiteSpace(parameter.parameterType) ? parameter.getType() : parameter.parameterType);
                        text_parameter = text_parameter.Replace("FW_PARAMETER_FIRST_UPPER", Utilities.FirstCharToUpper(parameter.name));
                        text_parameter = text_parameter.Replace("FW_PARAMETER", parameter.name);

                        parameters += text_parameter;
                    }

                    tags_controller.Add("FW_FRONT_CONTROLLER_PARAMETERS", parameters);

                    var controller_methods = controller.Components.Where(x => x.xsi_type == "frameweb:FrontControllerMethod").ToList();

                    string methods = string.Empty;
                    foreach (var method in controller_methods)
                    {
                        var text_method = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + method.getXsiTypeFile());
                        text_method = text_method.Replace("FW_METHOD_RETURN_TYPE", (string.IsNullOrWhiteSpace(method.methodType)) ? "void" : method.methodType);
                        text_method = text_method.Replace("FW_METHOD_NAME", method.name);

                        if(!string.IsNullOrWhiteSpace(method.methodType))
                            text_method = text_method.Replace("FW_METHOD_RETURN", "return null;");
                        else
                            text_method = text_method.Replace("FW_METHOD_RETURN", string.Empty);


                        text_method = text_method.Replace("FW_METHOD_PARAM", method.GetMethodParameter());                       

                        methods += text_method;
                    }

                    tags_controller.Add("FW_FRONT_CONTROLLER_METHODS", methods);

                    var text = File.ReadAllText(Config.dir_template + Config.lang + Path.DirectorySeparatorChar + controller.getXsiTypeFile());
                    foreach (var item in tags_controller)
                    {
                        text = text.Replace(item.Key, item.Value);
                    }

                    File.WriteAllText(Path.Combine(dir_output_class_package, controller.name + Config.ext_class), text);
                }
            }
            /// VIEW

            var views = componente.Components.Where(x => x.xsi_type == "frameweb:ViewPackage").ToList();
            foreach (var package_view in views)
            {
                var dir_output_page = this.BuildDirectoryStructures(Config.dir_output_web, package_view.name);


                var views_pages = package_view.Components.Where(x => x.xsi_type == "frameweb:Page").ToList();
                foreach (var page in views_pages)
                {
                    string body = string.Empty;

                    var componentes_dentro_pagina = page.Components.Where(x => x.xsi_type == "frameweb:NavigationCompositionWhole").ToList();
                    foreach (Component componente_pagina in componentes_dentro_pagina)
                    {
                        var comp = views.Where(x => x.name == componente_pagina.getType()).FirstOrDefault();
                        if (comp != null)
                        {
                            string body_form = string.Empty;
                            if (comp.xsi_type == "frameweb:UIComponent")// Form
                            {
                                body_form = File.ReadAllText(Config.dir_template + "framework" + Path.DirectorySeparatorChar + comp.getXsiTypeFile());
                            }
                            string body_form_comp = string.Empty;
                            foreach (var item in comp.Components)
                            {
                                // Get file path template for component UI (input, radio, ...)
                                var field_template = File.ReadAllText(item.getXsiTypeFile());
                                field_template = field_template.Replace("FW_ID", item.name.Replace('.', '_'));
                                field_template = field_template.Replace("FW_VALUE", comp.name + "." + item.name);

                                body_form_comp += field_template;
                            }

                            body_form = body_form.Replace("FW_BODY", body_form_comp);
                            body_form = body_form.Replace("FW_ID", comp.name);
                            body += body_form;
                        }
                    }

                    var text = File.ReadAllText(Config.dir_template + "framework" + Path.DirectorySeparatorChar + page.getXsiTypeFile());
                    text = text.Replace("FW_BODY", body);

                    File.WriteAllText(Path.Combine(dir_output_page, page.name), text);
                }
            }
            Utilities.Log("Code generated successfully.");

        }

    }
}

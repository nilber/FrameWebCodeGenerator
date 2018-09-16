using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorFrameweb
{
    public class Component
    {
        public string parameterType { get; set; }
        public string methodType { get; set; }
        public string tag { get; set; }
        public List<Component> Components { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string client { get; set; }
        public string supplier { get; set; }
        public string resultMethod { get; set; }
        public string association { get; set; }
        public string xsi_type { get; set; }
        public string memberEnd { get; set; }
        public string href { get; set; }
        public string visibility { get; set; }
        public bool isAbstract { get; set; }
        public string generalizationSet { get; set; }
        public string infix { get; set; }

        public string getXsiTypeFile()
        {
            switch (xsi_type)
            {
                case "frameweb:UIComponentField":
                    var tag_ui = this.Components.Where(x => x.tag == "type").FirstOrDefault();

                    //var parametros_ui = tag_ui.href.Split('/');
                    //return parametros_ui[parametros_ui.Length - 2] + "\\" + parametros_ui[parametros_ui.Length - 1] + ".txt";

                    try
                    {
                        var template = Program.PROFILE_BD[tag_ui.href.ToUpper()];
                        if (string.IsNullOrWhiteSpace(template))
                            throw new Exception("DEBUG: Tag codeGenerationTemplate is null or empty.");

                        if (!System.IO.File.Exists(template))
                            throw new Exception("DEBUG: File " + template + " not found.");
                        return template;
                    }
                    catch(Exception e)
                    {
                        // Console.Writeline and ReadKey just for DEBUG
                        if(e.Message.StartsWith("DEBUG"))
                        {
                            Console.WriteLine(e.Message);
                        }
                        else
                        {
                            Console.WriteLine("Tag codeGenerationTemplate not found.");
                        }
                        
                        Console.WriteLine("tag_ui.href =>" + tag_ui.href);
                        Console.WriteLine("Press any key to abort");
                        Console.ReadKey();
                        throw;
                    }

                case "frameweb:Page":
                    var tag_lib = this.Components.Where(x => x.tag == "pageTagLib").FirstOrDefault();

                    if (tag_lib != null)
                    {
                        var parametros = tag_lib.href.Split('/');
                        return parametros[parametros.Length - 1] + ".txt";
                    }
                    else
                    {
                        return "HtmlDefault.txt";
                    }
                default:
                    return this.xsi_type.Split(':')[1] + ".txt";
            }
        }

        internal void Process(Config config)
        {
            switch (this.xsi_type)
            {
                case "frameweb:NavigationModel":
                    new ProcessNavigationModel(config).Execute(this);
                    break;
                case "frameweb:EntityModel":
                    new ProcessEntityModel(config).Execute(this);
                    break;
                case "frameweb:ApplicationModel":
                    new ProcessApplicationModel(config).Execute(this);
                    break;
                case "frameweb:PersistenceModel":
                    new ProcessPersistenceModel(config).Execute(this);
                    break;

                default:
                    break;
            }
        }

        public string getType()
        {
            if (string.IsNullOrWhiteSpace(this.type))
            {
                if(this.Components.Count > 0 && !string.IsNullOrWhiteSpace(this.Components.First().href))
                {
                    var _href = this.Components.First().href.Split('/');
                    return _href[_href.Length - 1];
                }
                else
                {
                    return "No type";
                }
            }
            else
            {
                var parametros = this.type.Split('/');
                return parametros[parametros.Length - 1];
            }
        }

        internal string getClient()
        {
            var parametros = this.client.Split('/');
            return parametros[parametros.Length - 1];
        }

        internal string getSupplier()
        {
            var parametros = this.supplier.Split('/');
            return parametros[parametros.Length - 1];
        }

        internal string GetTypeDomainAttribute()
        {
            var value = string.Empty;

            if(this.Components != null)
            {
                var typeComponent = this.Components.Where(x => x.tag == "type").FirstOrDefault();
                if(typeComponent != null && !string.IsNullOrWhiteSpace(typeComponent.href))
                {
                    var _href = typeComponent.href.Split('/');
                    value = _href[_href.Length -1];
                }
            }
            return value;
        }
        internal string GetMethodTypeDomainAttribute()
        {
            var value = string.Empty;

            if (this.Components != null)
            {
                var typeComponent = this.Components.Where(x => x.tag == "methodType").FirstOrDefault();
                if (typeComponent != null && !string.IsNullOrWhiteSpace(typeComponent.href))
                {
                    var _href = typeComponent.href.Split('/');
                    value = _href[_href.Length - 1];
                }
            }
            return value;
        }

        internal string GetMethodParameter()
        {
            var value = string.Empty;

            if (this.Components != null)
            {
                foreach (var parameter in this.Components)
                {
                    if (parameter.tag == "ownedParameter" && parameter.Components != null && parameter.Components.Count > 0)
                    {
                        var typeComponent = parameter.Components.Where(x => x.tag == "type").FirstOrDefault();
                        if (typeComponent != null && !string.IsNullOrWhiteSpace(typeComponent.href))
                        {
                            var _href = typeComponent.href.Split('/');
                            value += _href[_href.Length - 1] + " " + parameter.name + ", ";
                        }
                    }
                    else if (parameter.tag == "ownedParameter" && !string.IsNullOrWhiteSpace(parameter.getType()))
                        value += parameter.getType() + " " + parameter.name + ", ";
                }

               
            }
            return string.IsNullOrWhiteSpace(value) ? value : value.Remove(value.Length - 2);
        }
    }
}

using System;
using System.IO;

namespace GeradorFrameweb
{
    public abstract class Process: IProcessor
    {
        public Config config { get; set; }

        public Process(Config _config){
            this.config = _config;
        }

        public abstract void Execute(Component componente);

        protected string BuildDirectoryStructures(string path_base, string path)
        {
            path = Path.Combine(config.dir_output, path_base, path).Replace('.', Path.DirectorySeparatorChar);
            Directory.CreateDirectory(path);

            return path;
        }
    }
}

﻿using System;
using System.IO;

namespace GeradorFrameweb
{
    public abstract class Processor: IProcessor
    {
        public Config Config { get; set; }

        protected Processor(Config _config){
            this.Config = _config;
        }

        public abstract void Execute(Component componente);

        protected string BuildDirectoryStructures(string path_base, string path)
        {
            path = Path.Combine(Config.dir_output, path_base, path).Replace('.', Path.DirectorySeparatorChar);
            Directory.CreateDirectory(path);

            return path;
        }
    }
}

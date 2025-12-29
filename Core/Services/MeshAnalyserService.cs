using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MeshAnalyserService : IMeshAnalyserService
    {
        public async Task<MeshDimensions> AnalyseAsync(string FilePath)
        {
            return await Task.Run(() => {
                var extension = Path.GetExtension(FilePath).ToLowerInvariant();

                return extension switch
                {
                    ".stl" => StlAnalysis(FilePath),
                    _ => throw new NotSupportedException($"Unsupported file type: {extension}")
                };
            });
        }

        private MeshDimensions StlAnalysis(string FilePath)
        {
            return new MeshDimensions(1,1,1);
        }
        //Repeat for other File Types
    }
}

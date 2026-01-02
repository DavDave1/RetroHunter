using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetroHunter.Services
{
    public class CompressProgress
    {
        public float Percent { get; set; }
    }
    public interface ICompressService
    {
        public Task CompressRom(UserConfig config, Rom rom, IProgress<CompressProgress>? progress = null);
    }
}

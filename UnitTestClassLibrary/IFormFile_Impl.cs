using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestClassLibrary
{
    public class IFormFile_Impl : IFormFile
    {
        public string ContentType => throw new System.NotImplementedException();

        public string ContentDisposition => throw new System.NotImplementedException();

        public IHeaderDictionary Headers => throw new System.NotImplementedException();

        public long Length => throw new System.NotImplementedException();

        public string Name => throw new System.NotImplementedException();

        public string FileName => throw new System.NotImplementedException();

        public void CopyTo(Stream target)
        {
            throw new System.NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return null;
        }

        public Stream OpenReadStream()
        {
            throw new System.NotImplementedException();
        }
    }
}
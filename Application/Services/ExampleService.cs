using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ExampleService : IExampleService
    {
        public async Task<string> ExampleReturnText(int id)
        {
            throw new NotImplementedException();
        }
    }
}

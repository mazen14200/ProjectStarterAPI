using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IExampleService
    {
        Task<string> ExampleReturnText(int id);
    }
}

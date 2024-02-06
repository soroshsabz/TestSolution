using BSN.Commons.Responses;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryTestMVC
{
   /// <summary>
    /// based on https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters#how-to-use---response-examples
    /// </summary>
    public class TestDataViewModelResponseErrorExample : IExamplesProvider<Response<TestDataViewModel>>
    {
        /// <summary>
        /// Get examples
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Response<TestDataViewModel> GetExamples() => new Response<TestDataViewModel>()
        {
            Data = null,
            InvalidItems = new List<InvalidItem>()
            {
                new InvalidItem()
                {
                    Name = "Id",
                    Reason = "Id is required"
                }
            },
            StatusCode = BSN.Commons.PresentationInfrastructure.ResponseStatusCode.BadRequest,
            Message = "Bad Request"
        };
    }

    /// <summary>
    /// based on https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters#how-to-use---response-examples
    /// </summary>
    public class TestDataViewModelResponseOkExample : IExamplesProvider<Response<TestDataViewModel>>
    {
        /// <summary>
        /// Get examples
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Response<TestDataViewModel> GetExamples() => new Response<TestDataViewModel>()
        {
            Data = new TestDataViewModel()
            {
                Id = 199,
                Name = "UUUUUUUUUU"
            },
            StatusCode = BSN.Commons.PresentationInfrastructure.ResponseStatusCode.OK,
            Message = "Ok"
        };
    }
}

using BSN.Commons.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AutofacHandyMVCTest.Controllers.V1
{
    /// <summary>
    /// Just for test swagger documentation
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SwaggerSampleTestController : ControllerBase
    {
        /// <summary>
        /// Add a new test data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="200">
        /// Returns the newly created item
        /// </response>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Response<TestDataViewModel>), 200)]
        [ProducesResponseType(typeof(Response<TestDataViewModel>), 400)]
        [SwaggerResponseExample(200, typeof(TestDataViewModelResponseOkExample))]
        [SwaggerResponseExample(400, typeof(TestDataViewModelResponseErrorExample))]
        [AllowAnonymous]
        public async Task<ActionResult<Response<TestDataViewModel>>> Add([FromBody] TestRequest request)
        {
            return await Task.FromResult(new Response<TestDataViewModel>()
            {
                Data = new TestDataViewModel()
                {
                    Id = 1,
                    Name = "Test"
                },
                StatusCode = BSN.Commons.PresentationInfrastructure.ResponseStatusCode.OK,
                Message = "OK"
            });
        }
    }

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

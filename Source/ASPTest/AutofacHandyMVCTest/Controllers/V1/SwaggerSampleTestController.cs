using BSN.Commons.Responses;
using ClassLibraryTestMVC;
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

 }

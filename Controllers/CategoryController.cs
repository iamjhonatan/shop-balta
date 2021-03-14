using Microsoft.AspNetCore.Mvc;

// https://localhost:5001 = porta padrao com o https
// http://localhost:5000 = porta padrao sem o https
// https://meuapp.azurewebsites.net/ = servidor
[Route("categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public string Get()
    {
        return "GET";
    }

    [HttpPost]
    [Route("")]
    public string Post()
    {
        return "POST";
    }

    [HttpPut]
    [Route("")]
    public string Put()
    {
        return "PUT";
    }

    [HttpDelete]
    [Route("")]
    public string Delete()
    {
        return "DELETE";
    }
}
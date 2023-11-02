

using API.Dtos;
using API.Helpers.Paginacion;
using API.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("1.1")]


public class UserController : ApiBaseController
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork unitofwork;
    private readonly IMapper mapper;
     private readonly ILogger<UserController> logger;
     private readonly IAuth auth;
     private readonly IPasswordHasher<User> _passwordHasher;

    public UserController(IUserService userService, IUnitOfWork unitofwork, IMapper mapper, IAuth auth,ILogger<UserController> logger, IPasswordHasher<User> passwordHasher)
    {
        _userService = userService;
        this.unitofwork = unitofwork;
        this.mapper = mapper;
        this.logger = logger;
        this.auth = auth;
         _passwordHasher = passwordHasher;
        

    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get()
    {
        var entidad = await unitofwork.Users.GetAllAsync();
        return mapper.Map<List<UserDto>>(entidad);
    }


    // [HttpGet]
    // [MapToApiVersion("1.1")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public async Task<ActionResult<Pager<UserDto>>> GetPaginacion([FromQuery] Params usuarioParams)
    // {
    //     var entidad = await unitofwork.Users.GetAllAsync(usuarioParams.PageIndex, usuarioParams.PageSize, usuarioParams.Search);
    //     var listEntidad = mapper.Map<List<UserDto>>(entidad.registros);
    //     return new Pager<UserDto>(listEntidad, entidad.totalRegistros, usuarioParams.PageIndex, usuarioParams.PageSize, usuarioParams.Search);
    // }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Get(int id)
    {
        var entidad = await unitofwork.Users.GetByIdAsync(id);
        if (entidad == null)
        {
            return NotFound();
        }
        return this.mapper.Map<UserDto>(entidad);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Put(int id, [FromBody] UserDto entidadDto)
    {
        if (entidadDto == null)
        {
            return NotFound();
        }
        var entidad = this.mapper.Map<User>(entidadDto);
        unitofwork.Users.Update(entidad);
        await unitofwork.SaveAsync();
        return entidadDto;
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var entidad = await unitofwork.Users.GetByIdAsync(id);
        if (entidad == null)
        {
            return NotFound();
        }
        unitofwork.Users.Remove(entidad);
        await unitofwork.SaveAsync();
        return NoContent();
    }


    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult> RegisterAsync(RegisterDto model)
    {
        var result = await _userService.RegisterAsync(model);
        return Ok(result);
    }


    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Login([FromBody] LoginDto data)
    {
        
            User user =  await unitofwork.Users.GetByUsernameAsync(data.Username);

            if (user == null)
            {
                return BadRequest("Usuario no válido.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, data.Password);

            if (result == PasswordVerificationResult.Success)
            {

            byte[] QR = auth.CreateQR(ref user);
            unitofwork.Users.Update(user);
            await unitofwork.SaveAsync();

            // Realizar la autenticación y obtener el token
            DataUserDto tokenData = await _userService.GetTokenAsync(data);
            SetRefreshTokenInCookie(tokenData.RefreshToken);

            // Envía el correo electrónico con el QR
            await auth.SendEmail(user, QR);

            return Ok(tokenData);
            }
            else
            {
                return BadRequest("Error, se produjo un error durante la autenticación.");
            }
    }




    [HttpPost("addrole")]
    public async Task<IActionResult> AddRoleAsync(AddRoleDto model)
    {
        var result = await _userService.AddRoleAsync(model);
        return Ok(result);
    }



    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _userService.RefreshTokenAsync(refreshToken);
        if (!string.IsNullOrEmpty(response.RefreshToken))
            SetRefreshTokenInCookie(response.RefreshToken);
        return Ok(response);
    }



    [HttpPost("VerifyCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]    
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    
        public async Task<ActionResult> Verify([FromBody] AuthDto data)
        {        
            try
            {
                User u = await unitofwork.Users.FindFirst(p => p.Username == data.Username);
                if(u.TwoStepSecret == null)
                {
                    throw new ArgumentNullException(u.TwoStepSecret);
                }
                var isVerified = auth.VerifyCode(u.TwoStepSecret, data.Code);            

                if(isVerified == true)
                {
                    return Ok("authenticated, checked");
                }
                return Unauthorized();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest("error, some error occurred");
            }                        
        }


    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10),
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    
    
}
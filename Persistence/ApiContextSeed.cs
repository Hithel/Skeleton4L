
using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Persistence;
    public class ApiContextSeed
    {
        public static async Task SeedAsync(ApiContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            var ruta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!context.Users.Any())
            {
                using (var readerEntity = new StreamReader("../Persistence/Data/Csvs/User.Csv"))
                {
                    using (var csv = new CsvReader(readerEntity, CultureInfo.InvariantCulture))
                    {
                        var list = csv.GetRecords<User>();
                        context.Users.AddRange(list);
                        await context.SaveChangesAsync();
                    }
                }
            } 


            //Clase que no tiene forenea de otra tabla

        //    if (!context.Especies.Any())
        //     {
        //         using (var readerEntity = new StreamReader("../Persistence/Data/Csvs/Especie.Csv"))
        //         {
        //             using (var csv = new CsvReader(readerEntity, CultureInfo.InvariantCulture))
        //             {
        //                 var list = csv.GetRecords<Especie>();
        //                 context.Especies.AddRange(list);
        //                 await context.SaveChangesAsync();
        //             }
        //         }
        //     }


            //Tabla que requiere una fornea

            // if (!context.Razas.Any())
            // {
            //     using (var reader = new StreamReader("../Persistence/Data/Csvs/Raza.Csv"))
            //     {
            //         using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            //         {
            //             HeaderValidated = null, // Esto deshabilita la validación de encabezados
            //             MissingFieldFound = null
            //         }))
            //         {
            //             // Resto de tu código para leer y procesar el archivo CSV
            //             var list = csv.GetRecords<Raza>();
            //             List<Raza> entidad = new List<Raza>();
            //             foreach (var item in list)
            //             {
            //                 entidad.Add(new Raza
            //                 {
            //                     Id = item.Id,
            //                     IdEspecieFK = item.IdEspecieFK,
            //                     Descripcion = item.Descripcion
            //                 });
            //             }
            //             context.Razas.AddRange(entidad);
            //             await context.SaveChangesAsync();
            //         }
            //     }
            // }





        

            if (!context.UsersRols.Any())
            {
                using (var reader = new StreamReader("../Persistence/Data/Csvs/UserRol.Csv"))
                {
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null, // Esto deshabilita la validación de encabezados
                        MissingFieldFound = null
                    }))
                    {
                        // Resto de tu código para leer y procesar el archivo CSV
                        var list = csv.GetRecords<UserRol>();
                        List<UserRol> entidad = new List<UserRol>();
                        foreach (var item in list)
                        {
                            entidad.Add(new UserRol
                            {
                                UsuarioId = item.UsuarioId,
                                RolId = item.RolId
                            });
                        }
                        context.UsersRols.AddRange(entidad);
                        await context.SaveChangesAsync();
                    }
                }
            }
             
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<ApiContext>();
            logger.LogError(ex.Message);
        }
    }
    public static async Task SeedRolesAsync(ApiContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            if (!context.Rols.Any())
            {
                var roles = new List<Rol>()
                        {
                            new Rol{Id=1, Nombre="Administrator"},
                            new Rol{Id=2, Nombre="Manager"},
                            new Rol{Id=3, Nombre="Employee"}
                        };
                context.Rols.AddRange(roles);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<ApiContext>();
            logger.LogError(ex.Message);
        }
    }
    }

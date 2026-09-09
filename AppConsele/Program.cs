using AccesoDatos.Data;
using AccesoDatos.Models;
using AccesoDatos.Repositories;

const decimal PRECIO_POR_DIA = 100m;
const decimal PORCENTAJE_RECARGO = 0.10m;

IGenericRepository<Pelicula> repoPeliculas = new GenericRepository<Pelicula>();
IGenericRepository<Socio> repoSocios = new GenericRepository<Socio>();
IGenericRepository<Alquiler> repoAlquileres = new GenericRepository<Alquiler>();
IGenericRepository<DetalleAlquiler> repoDetalles = new GenericRepository<DetalleAlquiler>();

bool salir = false;

while (!salir)
{
    Console.WriteLine("\n===== MENU VIDEOCLUB =====");
    Console.WriteLine("1. Registrar nueva pelicula");
    Console.WriteLine("2. Registrar nuevo socio");
    Console.WriteLine("3. Registrar un alquiler");
    Console.WriteLine("4. Registrar devolucion");
    Console.WriteLine("5. Reporte de alquiler por socio");
    Console.WriteLine("6. Reporte de socios con demora en devolucion");
    Console.WriteLine("7. Reporte de peliculas mas alquiladas");
    Console.WriteLine("8. Reporte del socio que mas peliculas alquilo");
    Console.WriteLine("0. Salir");
    Console.Write("Elegi una opcion: ");

    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            RegistrarPelicula();
            break;
        case "2":
            RegistrarSocio();
            break;
        case "3":
            RegistrarAlquiler();
            break;
        case "4":
            RegistrarDevolucion();
            break;
        case "5":
            ReporteAlquilerPorSocio();
            break;
        case "6":
            ReporteSociosConDemora();
            break;
        case "7":
            ReportePeliculasMasAlquiladas();
            break;
        case "8":
            ReporteSocioQueMasAlquilo();
            break;
        case "0":
            salir = true;
            Console.WriteLine("Saliendo...");
            break;
        default:
            Console.WriteLine("Opcion invalida, intenta de nuevo.");
            break;
    }
}

// ==================== 1. REGISTRAR PELICULA ====================
void RegistrarPelicula()
{
    Console.Write("Titulo: ");
    string titulo = Console.ReadLine();
    Console.Write("Autor: ");
    string autor = Console.ReadLine();
    Console.Write("Cantidad disponible: ");
    int cantidad = int.Parse(Console.ReadLine());

    repoPeliculas.Agregar(new Pelicula { Titulo = titulo, Autor = autor, CantidadDisponible = cantidad });
    Console.WriteLine("Pelicula registrada con exito.");
}

// ==================== 2. REGISTRAR SOCIO ====================
void RegistrarSocio()
{
    Console.Write("Nombre: ");
    string nombre = Console.ReadLine();
    Console.Write("Apellido: ");
    string apellido = Console.ReadLine();
    Console.Write("DNI: ");
    string dni = Console.ReadLine();
    Console.Write("Telefono: ");
    string telefono = Console.ReadLine();

    repoSocios.Agregar(new Socio { Nombre = nombre, Apellido = apellido, Dni = dni, Telefono = telefono });
    Console.WriteLine("Socio registrado con exito.");
}

// ==================== 3. REGISTRAR ALQUILER ====================
void RegistrarAlquiler()
{
    Console.Write("DNI del socio: ");
    string dni = Console.ReadLine();
    var socio = repoSocios.ObtenerTodos().FirstOrDefault(s => s.Dni == dni);

    if (socio == null)
    {
        Console.WriteLine("No se encontro un socio con ese DNI.");
        return;
    }

    Console.Write("Dias de alquiler: ");
    int dias = int.Parse(Console.ReadLine());

    var nuevoAlquiler = new Alquiler
    {
        SocioId = socio.Id,
        FechaAlquiler = DateTime.Now,
        FechaDevolucionPactada = DateTime.Now.AddDays(dias),
        FechaDevolucionReal = null
    };
    repoAlquileres.Agregar(nuevoAlquiler);

    bool agregarMas = true;
    while (agregarMas)
    {
        Console.Write("Titulo de la pelicula a alquilar: ");
        string titulo = Console.ReadLine();
        var pelicula = repoPeliculas.ObtenerTodos().FirstOrDefault(p => p.Titulo == titulo);

        if (pelicula == null)
        {
            Console.WriteLine("No se encontro esa pelicula.");
        }
        else if (pelicula.CantidadDisponible <= 0)
        {
            Console.WriteLine("No hay unidades disponibles de esa pelicula.");
        }
        else
        {
            decimal montoBase = PRECIO_POR_DIA * dias;

            var detalle = new DetalleAlquiler
            {
                AlquilerId = nuevoAlquiler.Id,
                PeliculaId = pelicula.Id,
                DiasAlquilados = dias,
                MontoBase = montoBase,
                Recargo = 0,
                MontoTotal = montoBase
            };
            repoDetalles.Agregar(detalle);

            // Descontar la cantidad disponible
            pelicula.CantidadDisponible -= 1;
            repoPeliculas.Actualizar(pelicula);

            Console.WriteLine($"'{pelicula.Titulo}' agregada al alquiler. Monto: ${montoBase}");
        }

        Console.Write("Agregar otra pelicula a este alquiler? (s/n): ");
        agregarMas = Console.ReadLine().Trim().ToLower() == "s";
    }

    Console.WriteLine("Alquiler registrado con exito.");
}

// ==================== 4. REGISTRAR DEVOLUCION ====================
void RegistrarDevolucion()
{
    Console.Write("ID del alquiler a devolver: ");
    int alquilerId = int.Parse(Console.ReadLine());

    var alquiler = repoAlquileres.ObtenerTodos().FirstOrDefault(a => a.Id == alquilerId);
    if (alquiler == null)
    {
        Console.WriteLine("No se encontro ese alquiler.");
        return;
    }

    if (alquiler.FechaDevolucionReal != null)
    {
        Console.WriteLine("Ese alquiler ya fue devuelto.");
        return;
    }

    alquiler.FechaDevolucionReal = DateTime.Now;

    int diasDemora = 0;
    if (alquiler.FechaDevolucionReal > alquiler.FechaDevolucionPactada)
    {
        diasDemora = (alquiler.FechaDevolucionReal.Value - alquiler.FechaDevolucionPactada).Days;
    }

    repoAlquileres.Actualizar(alquiler);

    var detalles = repoDetalles.ObtenerTodos().Where(d => d.AlquilerId == alquiler.Id).ToList();
    var peliculas = repoPeliculas.ObtenerTodos();

    foreach (var detalle in detalles)
    {
        if (diasDemora > 0)
        {
            // 10% mas por cada dia de demora, sobre el monto base
            detalle.Recargo = detalle.MontoBase * PORCENTAJE_RECARGO * diasDemora;
            detalle.MontoTotal = detalle.MontoBase + detalle.Recargo;
            repoDetalles.Actualizar(detalle);
        }

        // Devolver la unidad al stock disponible
        var pelicula = peliculas.FirstOrDefault(p => p.Id == detalle.PeliculaId);
        if (pelicula != null)
        {
            pelicula.CantidadDisponible += 1;
            repoPeliculas.Actualizar(pelicula);
        }
    }

    if (diasDemora > 0)
        Console.WriteLine($"Devolucion registrada con {diasDemora} dia(s) de demora. Se aplico recargo.");
    else
        Console.WriteLine("Devolucion registrada en tiempo y forma.");
}

// ==================== 5. REPORTE ALQUILER POR SOCIO ====================
void ReporteAlquilerPorSocio()
{
    var socios = repoSocios.ObtenerTodos();
    var alquileres = repoAlquileres.ObtenerTodos();
    var detalles = repoDetalles.ObtenerTodos();
    var peliculas = repoPeliculas.ObtenerTodos();

    Console.WriteLine("\n--- ALQUILERES ACTUALES POR SOCIO ---");
    foreach (var socio in socios)
    {
        Console.WriteLine($"\nSocio: {socio.Nombre} {socio.Apellido} (DNI: {socio.Dni})");

        var alquileresActivos = alquileres.Where(a => a.SocioId == socio.Id && a.FechaDevolucionReal == null).ToList();

        if (!alquileresActivos.Any())
        {
            Console.WriteLine("  Sin peliculas alquiladas actualmente.");
            continue;
        }

        foreach (var alq in alquileresActivos)
        {
            var detallesDelAlquiler = detalles.Where(d => d.AlquilerId == alq.Id);
            foreach (var det in detallesDelAlquiler)
            {
                var peli = peliculas.FirstOrDefault(p => p.Id == det.PeliculaId);
                Console.WriteLine($"  - {peli?.Titulo} | Vence: {alq.FechaDevolucionPactada:dd/MM/yyyy}");
            }
        }
    }
}

// ==================== 6. REPORTE SOCIOS CON DEMORA ====================
void ReporteSociosConDemora()
{
    var socios = repoSocios.ObtenerTodos();
    var alquileres = repoAlquileres.ObtenerTodos();

    Console.WriteLine("\n--- SOCIOS CON DEMORA EN DEVOLUCION ---");

    var conDemora = alquileres.Where(a =>
        // Si no fue devuelto y la fecha pactada ya pasó -> demora
        (a.FechaDevolucionReal == null && DateTime.Now > a.FechaDevolucionPactada) ||
        // Si fue devuelto y la fecha real es posterior a la pactada -> hubo demora
        (a.FechaDevolucionReal != null && a.FechaDevolucionReal > a.FechaDevolucionPactada)
    ).ToList();

    if (!conDemora.Any())
    {
        Console.WriteLine("No hay socios con demora.");
        return;
    }

    foreach (var alq in conDemora)
    {
        var socio = socios.FirstOrDefault(s => s.Id == alq.SocioId);
        var fechaComparar = alq.FechaDevolucionReal ?? DateTime.Now;
        int diasDemora = (fechaComparar - alq.FechaDevolucionPactada).Days;

        Console.WriteLine($"{socio?.Nombre} {socio?.Apellido} | Alquiler #{alq.Id} | Demora: {diasDemora} dia(s)");
    }
}

// ==================== 7. REPORTE PELICULAS MAS ALQUILADAS ====================
void ReportePeliculasMasAlquiladas()
{
    var detalles = repoDetalles.ObtenerTodos();
    var peliculas = repoPeliculas.ObtenerTodos();

    Console.WriteLine("\n--- PELICULAS MAS ALQUILADAS ---");

    var ranking = detalles
        .GroupBy(d => d.PeliculaId)
        .Select(g => new { PeliculaId = g.Key, Cantidad = g.Count() })
        .OrderByDescending(x => x.Cantidad)
        .ToList();

    if (!ranking.Any())
    {
        Console.WriteLine("Todavia no hay alquileres registrados.");
        return;
    }

    foreach (var item in ranking)
    {
        var peli = peliculas.FirstOrDefault(p => p.Id == item.PeliculaId);
        Console.WriteLine($"{peli?.Titulo} | Veces alquilada: {item.Cantidad}");
    }
}

// ==================== 8. REPORTE SOCIO QUE MAS ALQUILO ====================
void ReporteSocioQueMasAlquilo()
{
    var alquileres = repoAlquileres.ObtenerTodos();
    var detalles = repoDetalles.ObtenerTodos();
    var socios = repoSocios.ObtenerTodos();

    Console.WriteLine("\n--- SOCIO QUE MAS PELICULAS ALQUILO ---");

    var ranking = alquileres
        .Select(a => new
        {
            a.SocioId,
            CantidadPeliculas = detalles.Count(d => d.AlquilerId == a.Id)
        })
        .GroupBy(x => x.SocioId)
        .Select(g => new { SocioId = g.Key, Total = g.Sum(x => x.CantidadPeliculas) })
        .OrderByDescending(x => x.Total)
        .FirstOrDefault();

    if (ranking == null || ranking.Total == 0)
    {
        Console.WriteLine("Todavia no hay alquileres registrados.");
        return;
    }

    var socio = socios.FirstOrDefault(s => s.Id == ranking.SocioId);
    Console.WriteLine($"{socio?.Nombre} {socio?.Apellido} con {ranking.Total} pelicula(s) alquilada(s) en total.");
}

Console.WriteLine("\nPresiona cualquier tecla para salir...");
Console.ReadKey();
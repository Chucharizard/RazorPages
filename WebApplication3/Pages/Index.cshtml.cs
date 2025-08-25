using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Models;
using System.Text.Json;

namespace WebApplication3.Pages
{
    public class IndexModel : PageModel
    {
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 5;
        public string EstadoFiltro { get; set; } = "";
        public string Mensaje { get; set; } = "";
        public bool MostrarFormulario { get; set; } = false;

        public void OnGet(int pagina = 1, string estado = "", int tamanoPagina = 5, 
                         bool mostrarForm = false, string accion = "", 
                         string nuevoNombre = "", string nuevaFecha = "")
        {
            MostrarFormulario = mostrarForm;
 
            if (accion == "crear" && !string.IsNullOrWhiteSpace(nuevoNombre) && !string.IsNullOrWhiteSpace(nuevaFecha))
            {
                var nuevaTarea = new Tarea
                {
                    nombreTarea = nuevoNombre.Trim(),
                    fechaVencimiento = nuevaFecha,
                    estado = "Pendiente"
                };

                if (GuardarNuevaTarea(nuevaTarea))
                {
                    Mensaje = "Tarea creada exitosamente";
                    MostrarFormulario = false;
                }
                else
                {
                    Mensaje = "Error al guardar la tarea";
                }
            }

            CargarTareas(pagina, estado, tamanoPagina);
        }

        private void CargarTareas(int pagina, string estado, int tamanoPagina)
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");
            var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            var todasLasTareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent) ?? new List<Tarea>();

            EstadoFiltro = estado;
            if (!string.IsNullOrWhiteSpace(estado))
            {
                todasLasTareas = todasLasTareas.Where(t => t.estado == estado).ToList();
            }
            else
            {
  
                todasLasTareas = todasLasTareas
                    .Where(t => t.estado == "Pendiente" || t.estado == "En curso")
                    .ToList();
            }

            TamanoPagina = tamanoPagina < 1 ? 5 : tamanoPagina;
            PaginaActual = pagina < 1 ? 1 : pagina;

       
            TotalPaginas = (int)Math.Ceiling(todasLasTareas.Count / (double)TamanoPagina);
            
            if (PaginaActual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaActual = TotalPaginas;
            }

            Tareas = todasLasTareas
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();
        }

        private bool GuardarNuevaTarea(Tarea nuevaTarea)
        {
            try
            {
                string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tareas.json");
                
                var jsonContent = System.IO.File.ReadAllText(jsonFilePath);
                var tareas = JsonSerializer.Deserialize<List<Tarea>>(jsonContent) ?? new List<Tarea>();
                
                tareas.Add(nuevaTarea);
                
                var nuevoJson = JsonSerializer.Serialize(tareas, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                
                System.IO.File.WriteAllText(jsonFilePath, nuevoJson);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

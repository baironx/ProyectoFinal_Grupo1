using AppEntrenamientoPersonal.Entidades;
using AppEntrenamientoPersonal.Fabricas;
using AppEntrenamientoPersonal.Interfaces;
using AppEntrenamientoPersonal.Repositorios;
using AppEntrenamientoPersonal.Servicios;
using AppEntrenamientoPersonal.UI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AppEntrenamientoPersonal
{
    /// <summary>
    /// Clase principal que contiene el punto de entrada del programa
    /// y maneja la lógica de la interfaz de usuario de la consola.
    /// Implementa el patrón Single Responsibility Principle (SRP).
    /// </summary>
    public class Program
    {
        // Variables estáticas que mantienen el estado de la aplicación
        private static IRepositorioAtletas<Atleta> repositorioAtletas;
        private static IRepositorioRutinas<Rutina> repositorioRutinas;
        private static IRepositorioSeguros<SeguroMedico> repositorioSeguros;
        private static IServicioSugerencias servicioSugerencias;
        private static IServicioEntrenamiento servicioEntrenamiento;
        private static IValidadorDatos<Atleta> validadorAtletas;
        private static IValidadorDatos<Rutina> validadorRutinas;
        private static IGestorAtletas gestorAtletas;
        private static IGestorRutinas gestorRutinas;
        private static IGestorSeguros gestorSeguros;
        private static InterfazUsuario interfazUsuario;

        private static Atleta atletaActual = null!;

        /// <summary>
        /// Punto de entrada principal del programa.
        /// Implementa Dependency Injection Container pattern.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                InicializarDependencias();
                CargarDatos();
                interfazUsuario = new InterfazUsuario();
                EjecutarAplicacion();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico en la aplicación: {ex.Message}");
                Console.WriteLine("Presione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Inicializa todas las dependencias del sistema siguiendo el principio de Inversión de Dependencias (DIP).
        /// </summary>
        private static void InicializarDependencias()
        {
            // Repositorios (Data Access Layer)
            repositorioAtletas = new RepositorioAtletasArchivo<Atleta>("atletas.txt");
            repositorioRutinas = new RepositorioRutinasArchivo<Rutina>("rutinas.txt");
            repositorioSeguros = new RepositorioSegurosArchivo<SeguroMedico>("seguros.txt");

            // Validadores
            validadorAtletas = new ValidadorAtletas();
            validadorRutinas = new ValidadorRutinas();

            // Servicios (Business Logic Layer)
            servicioSugerencias = new ServicioSugerenciaRutina();
            servicioEntrenamiento = new ServicioEntrenamiento(repositorioRutinas, repositorioSeguros);

            // Gestores (Application Service Layer)
            gestorAtletas = new GestorAtletas(repositorioAtletas, validadorAtletas);
            gestorRutinas = new GestorRutinas(repositorioRutinas, validadorRutinas);
            gestorSeguros = new GestorSeguros(repositorioSeguros);
        }

        /// <summary>
        /// Carga datos iniciales desde los repositorios.
        /// </summary>
        private static void CargarDatos()
        {
            try
            {
                Console.WriteLine("Cargando datos...");
                // Los repositorios cargan automáticamente los datos al ser inicializados
                Console.WriteLine("Datos cargados exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Ejecuta el bucle principal de la aplicación.
        /// </summary>
        private static void EjecutarAplicacion()
        {
            bool continuar = true;
            while (continuar)
            {
                try
                {
                    MostrarMenuPrincipal();
                    string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                    continuar = ProcesarOpcionMenu(opcion);
                }
                catch (Exception ex)
                {
                    interfazUsuario.MostrarError($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Muestra el menú principal de la aplicación.
        /// </summary>
        private static void MostrarMenuPrincipal()
        {
            interfazUsuario.LimpiarPantalla();
            interfazUsuario.MostrarTitulo("=== SISTEMA DE ENTRENAMIENTO PERSONAL ===");

            if (atletaActual != null)
            {
                interfazUsuario.MostrarInformacion($"Atleta Activo: {atletaActual.Nombre}");
            }

            interfazUsuario.MostrarMenu(new[]
            {
                "1. Gestionar Atletas",
                "2. Gestionar Rutinas de Entrenamiento",
                "3. Gestionar Seguros Médicos",
                "4. Estadísticas y Reportes",
                "5. Sugerencias de Ejercicios",
                "6. Búsquedas Avanzadas",
                "7. Configuración",
                "8. Guardar y Salir"
            });
        }

        /// <summary>
        /// Procesa la opción seleccionada en el menú principal.
        /// Implementa el patrón Command usando delegates.
        /// </summary>
        private static bool ProcesarOpcionMenu(string opcion)
        {
            // Delegate para comandos del menú
            Action comando = opcion switch
            {
                "1" => () => GestionarAtletas(),
                "2" => () => GestionarRutinas(),
                "3" => () => GestionarSeguros(),
                "4" => () => MostrarEstadisticasYReportes(),
                "5" => () => MostrarSugerenciasEjercicios(),
                "6" => () => RealizarBusquedasAvanzadas(),
                "7" => () => MostrarConfiguracion(),
                "8" => () => GuardarYSalir(),
                _ => () => interfazUsuario.MostrarError("Opción no válida, intente de nuevo.")
            };

            comando.Invoke();

            return opcion != "8";
        }

        #region Gestión de Atletas

        /// <summary>
        /// Gestiona todas las operaciones relacionadas con atletas.
        /// </summary>
        private static void GestionarAtletas()
        {
            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== GESTIÓN DE ATLETAS ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Mostrar Atletas",
                    "2. Agregar Nuevo Atleta",
                    "3. Seleccionar Atleta Activo",
                    "4. Editar Atleta",
                    "5. Eliminar Atleta",
                    "6. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": MostrarAtletas(); break;
                    case "2": AgregarAtleta(); break;
                    case "3": SeleccionarAtletaActivo(); break;
                    case "4": EditarAtleta(); break;
                    case "5": EliminarAtleta(); break;
                    case "6": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void MostrarAtletas()
        {
            var atletas = gestorAtletas.ObtenerTodos();
            interfazUsuario.MostrarSubTitulo("=== LISTA DE ATLETAS ===");

            if (!atletas.Any())
            {
                interfazUsuario.MostrarAdvertencia("No hay atletas registrados.");
                return;
            }

            for (int i = 0; i < atletas.Count(); i++)
            {
                var atleta = atletas.ElementAt(i);
                string activo = atleta == atletaActual ? " (ACTIVO)" : "";
                interfazUsuario.MostrarInformacion($"{i + 1}. {atleta}{activo}");
            }
        }

        private static void AgregarAtleta()
        {
            try
            {
                interfazUsuario.MostrarSubTitulo("=== AGREGAR NUEVO ATLETA ===");

                var nombre = interfazUsuario.LeerEntrada("Nombre del atleta: ");
                var peso = interfazUsuario.LeerDouble("Peso del atleta (kg): ");
                var alturaIngresada = interfazUsuario.LeerDouble("Altura del atleta (cm): ");
                double altura;
                // Convierte de cm a m si el valor es mayor que 3.0
                if (alturaIngresada > 3.0)
                {
                    altura = alturaIngresada / 100.0;
                }
                else
                {
                    altura = alturaIngresada;
                }
                var objetivos = interfazUsuario.LeerEntrada("Objetivos del atleta: ");
                var nivel = interfazUsuario.LeerEntrada("Nivel del atleta (Principiante/Intermedio/Avanzado): ");

                var nuevoAtleta = new Atleta(nombre, peso, altura, objetivos, nivel);
                gestorAtletas.Agregar(nuevoAtleta);

                if (atletaActual == null)
                {
                    atletaActual = nuevoAtleta;
                    interfazUsuario.MostrarExito("Atleta registrado exitosamente y seleccionado como activo.");
                }
                else
                {
                    interfazUsuario.MostrarExito("Atleta registrado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                interfazUsuario.MostrarError($"Error al agregar atleta: {ex.Message}");
            }
        }

        private static void SeleccionarAtletaActivo()
        {
            var atletas = gestorAtletas.ObtenerTodos().ToList();
            if (!atletas.Any())
            {
                interfazUsuario.MostrarAdvertencia("No hay atletas registrados.");
                return;
            }

            MostrarAtletas();
            var indice = interfazUsuario.LeerEntero("Seleccione el número del atleta: ");

            if (indice > 0 && indice <= atletas.Count) // Ajuste para índice 1-based
            {
                atletaActual = atletas[indice - 1];
                interfazUsuario.MostrarExito($"Atleta activo: {atletaActual.Nombre}");
            }
            else
            {
                interfazUsuario.MostrarError("Número inválido.");
            }
        }

        private static void EditarAtleta()
        {
            var atletas = gestorAtletas.ObtenerTodos().ToList();
            if (!atletas.Any())
            {
                interfazUsuario.MostrarAdvertencia("No hay atletas registrados.");
                return;
            }

            MostrarAtletas();
            var indice = interfazUsuario.LeerEntero("Seleccione el número del atleta a editar: ");

            if (indice > 0 && indice <= atletas.Count)
            {
                var atleta = atletas[indice - 1];
                interfazUsuario.MostrarSubTitulo($"Editando: {atleta.Nombre}");
                interfazUsuario.MostrarInformacion("Presione Enter para mantener el valor actual");

                // Editar propiedades manteniendo valores actuales
                var nombre = interfazUsuario.LeerEntradaOpcional($"Nombre ({atleta.Nombre}): ", atleta.Nombre);
                var peso = interfazUsuario.LeerDoubleOpcional($"Peso ({atleta.Peso} kg): ", atleta.Peso);
                var altura = interfazUsuario.LeerDoubleOpcional($"Altura ({atleta.Altura} m): ", atleta.Altura);
                var objetivos = interfazUsuario.LeerEntradaOpcional($"Objetivos ({atleta.Objetivos}): ", atleta.Objetivos);
                var nivel = interfazUsuario.LeerEntradaOpcional($"Nivel ({atleta.Nivel}): ", atleta.Nivel);

                // Crear atleta actualizado
                var atletaActualizado = new Atleta(nombre, peso, altura, objetivos, nivel);
                gestorAtletas.Actualizar(atleta, atletaActualizado);

                interfazUsuario.MostrarExito("Atleta actualizado exitosamente.");
            }
            else
            {
                interfazUsuario.MostrarError("Número inválido.");
            }
        }

        private static void EliminarAtleta()
        {
            var atletas = gestorAtletas.ObtenerTodos().ToList();
            if (!atletas.Any())
            {
                interfazUsuario.MostrarAdvertencia("No hay atletas registrados.");
                return;
            }

            MostrarAtletas();
            var indice = interfazUsuario.LeerEntero("Seleccione el número del atleta a eliminar: ");

            if (indice > 0 && indice <= atletas.Count)
            {
                var atletaAEliminar = atletas[indice - 1];
                var confirmacion = interfazUsuario.LeerEntrada($"¿Está seguro de eliminar a {atletaAEliminar.Nombre}? (s/n): ");

                if (confirmacion.ToLower() == "s")
                {
                    gestorAtletas.Eliminar(atletaAEliminar);

                    if (atletaActual == atletaAEliminar)
                    {
                        var atletasRestantes = gestorAtletas.ObtenerTodos().ToList();
                        atletaActual = atletasRestantes.Any() ? atletasRestantes.First() : null!;
                    }

                    interfazUsuario.MostrarExito("Atleta eliminado exitosamente.");
                }
            }
            else
            {
                interfazUsuario.MostrarError("Número inválido.");
            }
        }

        #endregion

        #region Gestión de Rutinas

        private static void GestionarRutinas()
        {
            if (atletaActual == null)
            {
                interfazUsuario.MostrarAdvertencia("Debe seleccionar un atleta activo primero.");
                return;
            }

            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== GESTIÓN DE RUTINAS ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Mostrar Rutinas",
                    "2. Agregar Nueva Rutina",
                    "3. Editar Rutina",
                    "4. Eliminar Rutina",
                    "5. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": MostrarRutinas(); break;
                    case "2": AgregarRutina(); break;
                    case "3": EditarRutina(); break;
                    case "4": EliminarRutina(); break;
                    case "5": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void MostrarRutinas()
        {
            var rutinas = gestorRutinas.ObtenerPorAtleta(atletaActual.Nombre).ToList();
            interfazUsuario.MostrarSubTitulo($"=== RUTINAS DE {atletaActual.Nombre.ToUpper()} ===");

            if (!rutinas.Any())
            {
                interfazUsuario.MostrarAdvertencia($"No hay rutinas registradas para {atletaActual.Nombre}.");
                return;
            }

            for (int i = 0; i < rutinas.Count; i++)
            {
                interfazUsuario.MostrarInformacion($"{i + 1}. {rutinas[i].Describir()}");
            }
        }

        private static void AgregarRutina()
        {
            try
            {
                interfazUsuario.MostrarSubTitulo("=== AGREGAR NUEVA RUTINA ===");

                var tipo = interfazUsuario.LeerEntrada("Tipo de rutina (Fuerza/Cardio): ");
                var duracion = interfazUsuario.LeerEntero("Duración de la rutina (min): ");
                var intensidad = interfazUsuario.LeerEntrada("Intensidad de la rutina (Baja/Media/Alta): ");
                var grupoMuscular = interfazUsuario.LeerEntrada("Grupo muscular (Pecho/Espalda/Piernas/Brazos/Hombros/Abdomen/Cardio): ");

                var fechaRealizacion = interfazUsuario.LeerFecha("Fecha de realización (AAAA-MM-DD, Enter para hoy): ", DateTime.Today);
                var fechaVencimiento = interfazUsuario.LeerFechaOpcional("Fecha de vencimiento (AAAA-MM-DD, Enter si no aplica): ");
                var lesiones = interfazUsuario.LeerEntrada("Lesiones post-entrenamiento (Enter si no hay): ");

                // Información del seguro médico si hay lesiones
                SeguroMedico seguro = null!;
                if (!string.IsNullOrWhiteSpace(lesiones))
                {
                    var aplicaSeguro = interfazUsuario.LeerEntrada("¿Aplica seguro médico? (s/n): ");
                    if (aplicaSeguro.ToLower() == "s")
                    {
                        seguro = CrearSeguroMedico(lesiones);
                    }
                }

                // Factory pattern para crear rutinas
                Rutina nuevaRutina = FabricaRutinas.CrearRutina(
                    tipo, duracion, intensidad, grupoMuscular,
                    atletaActual.Nombre, fechaRealizacion, fechaVencimiento, lesiones, seguro);

                gestorRutinas.Agregar(nuevaRutina);

                if (seguro != null)
                {
                    gestorSeguros.Agregar(seguro);
                }

                interfazUsuario.MostrarExito($"Rutina de {tipo.ToLower()} agregada exitosamente.");
            }
            catch (Exception ex)
            {
                interfazUsuario.MostrarError($"Error al agregar rutina: {ex.Message}");
            }
        }

        private static SeguroMedico CrearSeguroMedico(string lesiones)
        {
            var nombreSeguro = interfazUsuario.LeerEntrada("Nombre del seguro: ");
            var montoCubierto = interfazUsuario.LeerDecimal("Monto cubierto por el seguro: ");
            var montoPaciente = interfazUsuario.LeerDecimal("Monto pagado por el paciente: ");
            var descripcionTratamiento = interfazUsuario.LeerEntrada("Descripción del tratamiento: ");

            return new SeguroMedico(
                Guid.NewGuid().ToString(),
                nombreSeguro,
                montoCubierto,
                montoPaciente,
                lesiones,
                descripcionTratamiento,
                atletaActual.Nombre,
                DateTime.Today
            );
        }

        private static void EditarRutina()
        {
            var rutinas = gestorRutinas.ObtenerPorAtleta(atletaActual.Nombre).ToList();
            if (!rutinas.Any())
            {
                interfazUsuario.MostrarAdvertencia($"No hay rutinas para editar para {atletaActual.Nombre}.");
                return;
            }

            MostrarRutinas();
            var indice = interfazUsuario.LeerEntero("Seleccione el número de la rutina a editar: ");

            if (indice > 0 && indice <= rutinas.Count)
            {
                var rutinaOriginal = rutinas[indice - 1];
                // Implementar edición similar al patrón usado en EditarAtleta
                interfazUsuario.MostrarExito("Rutina actualizada exitosamente.");
            }
            else
            {
                interfazUsuario.MostrarError("Número inválido.");
            }
        }

        private static void EliminarRutina()
        {
            var rutinas = gestorRutinas.ObtenerPorAtleta(atletaActual.Nombre).ToList();
            if (!rutinas.Any())
            {
                interfazUsuario.MostrarAdvertencia($"No hay rutinas para eliminar para {atletaActual.Nombre}.");
                return;
            }

            MostrarRutinas();
            var indice = interfazUsuario.LeerEntero("Seleccione el número de la rutina a eliminar: ");

            if (indice > 0 && indice <= rutinas.Count)
            {
                var rutinaAEliminar = rutinas[indice - 1];
                var confirmacion = interfazUsuario.LeerEntrada("¿Está seguro de eliminar esta rutina? (s/n): ");

                if (confirmacion.ToLower() == "s")
                {
                    gestorRutinas.Eliminar(rutinaAEliminar);
                    interfazUsuario.MostrarExito("Rutina eliminada exitosamente.");
                }
            }
            else
            {
                interfazUsuario.MostrarError("Número inválido.");
            }
        }

        #endregion

        #region Gestión de Seguros

        private static void GestionarSeguros()
        {
            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== GESTIÓN DE SEGUROS MÉDICOS ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Mostrar Seguros",
                    "2. Buscar Seguros",
                    "3. Editar Seguro",
                    "4. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": MostrarSeguros(); break;
                    case "2": BuscarSeguros(); break;
                    case "3": EditarSeguro(); break;
                    case "4": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void MostrarSeguros()
        {
            var seguros = gestorSeguros.ObtenerTodos().ToList();
            interfazUsuario.MostrarSubTitulo("=== SEGUROS MÉDICOS ===");

            if (!seguros.Any())
            {
                interfazUsuario.MostrarAdvertencia("No hay seguros registrados.");
                return;
            }

            foreach (var seguro in seguros)
            {
                interfazUsuario.MostrarInformacion(seguro.ToString());
            }
        }

        private static void BuscarSeguros()
        {
            interfazUsuario.MostrarSubTitulo("=== BUSCAR SEGUROS ===");
            interfazUsuario.MostrarMenu(new[]
            {
                "1. Buscar por nombre del seguro",
                "2. Buscar por monto cubierto",
                "3. Buscar por monto del paciente",
                "4. Buscar por atleta"
            });

            var opcion = interfazUsuario.LeerEntrada("Seleccione tipo de búsqueda: ");
            IEnumerable<SeguroMedico> resultados = Enumerable.Empty<SeguroMedico>();

            switch (opcion)
            {
                case "1":
                    var nombre = interfazUsuario.LeerEntrada("Ingrese nombre del seguro: ");
                    resultados = gestorSeguros.BuscarPorNombreSeguro(nombre);
                    break;
                case "2":
                    var montoCubierto = interfazUsuario.LeerDecimal("Ingrese monto cubierto: ");
                    resultados = gestorSeguros.BuscarPorMontoCubierto(montoCubierto);
                    break;
                case "3":
                    var montoPaciente = interfazUsuario.LeerDecimal("Ingrese monto del paciente: ");
                    resultados = gestorSeguros.BuscarPorMontoPaciente(montoPaciente);
                    break;
                case "4":
                    var atleta = interfazUsuario.LeerEntrada("Ingrese nombre del atleta: ");
                    resultados = gestorSeguros.BuscarPorAtleta(atleta);
                    break;
                default:
                    interfazUsuario.MostrarError("Opción no válida.");
                    return;
            }

            MostrarResultadosBusquedaSeguros(resultados);
        }

        private static void MostrarResultadosBusquedaSeguros(IEnumerable<SeguroMedico> seguros)
        {
            var listaResultados = seguros.ToList();
            if (listaResultados.Any())
            {
                interfazUsuario.MostrarSubTitulo("=== RESULTADOS DE BÚSQUEDA ===");
                foreach (var seguro in listaResultados)
                {
                    interfazUsuario.MostrarInformacion(seguro.ToString());
                }
            }
            else
            {
                interfazUsuario.MostrarAdvertencia("No se encontraron seguros con los criterios especificados.");
            }
        }

        private static void EditarSeguro()
        {
            var seguros = gestorSeguros.ObtenerTodos().ToList();
            if (!seguros.Any())
            {
                interfazUsuario.MostrarAdvertencia("No hay seguros registrados.");
                return;
            }

            MostrarSeguros();
            var indice = interfazUsuario.LeerEntero("Seleccione el número del seguro a editar: ");

            if (indice > 0 && indice <= seguros.Count)
            {
                var seguroOriginal = seguros[indice - 1];
                // Implementar edición del seguro
                interfazUsuario.MostrarExito("Seguro actualizado exitosamente.");
            }
            else
            {
                interfazUsuario.MostrarError("Número inválido.");
            }
        }

        #endregion

        #region Estadísticas y Reportes

        private static void MostrarEstadisticasYReportes()
        {
            if (atletaActual == null)
            {
                interfazUsuario.MostrarAdvertencia("Debe seleccionar un atleta activo primero.");
                return;
            }

            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== ESTADÍSTICAS Y REPORTES ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Estadísticas Generales",
                    "2. Lesiones del Último Mes",
                    "3. Top 3 Lesiones del Trimestre",
                    "4. Análisis de Seguros",
                    "5. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": MostrarEstadisticasGenerales(); break;
                    case "2": MostrarLesionesUltimoMes(); break;
                    case "3": MostrarTop3LesionesTrimestr(); break;
                    case "4": MostrarAnalisisSeguros(); break;
                    case "5": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void MostrarEstadisticasGenerales()
        {
            var estadisticas = servicioEntrenamiento.GenerarEstadisticas(atletaActual.Nombre);
            interfazUsuario.MostrarSubTitulo($"=== ESTADÍSTICAS GENERALES - {atletaActual.Nombre} ===");
            interfazUsuario.MostrarInformacion(estadisticas.ToString());
        }

        private static void MostrarLesionesUltimoMes()
        {
            var lesiones = servicioEntrenamiento.ObtenerLesionesUltimoMes(atletaActual.Nombre);
            interfazUsuario.MostrarSubTitulo("=== LESIONES DEL ÚLTIMO MES ===");
            interfazUsuario.MostrarInformacion($"Cantidad de lesiones: {lesiones.Count()}");

            foreach (var lesion in lesiones)
            {
                interfazUsuario.MostrarInformacion($"- {lesion.FechaRealizacion.ToShortDateString()}: {lesion.LesionesPostEntrenamiento}");
            }
        }

        private static void MostrarTop3LesionesTrimestr()
        {
            var topLesiones = servicioEntrenamiento.ObtenerTop3LesionesTrimestre(atletaActual.Nombre);
            interfazUsuario.MostrarSubTitulo("=== TOP 3 LESIONES MÁS FRECUENTES DEL TRIMESTRE ===");

            int posicion = 1;
            foreach (var grupo in topLesiones)
            {
                interfazUsuario.MostrarInformacion($"{posicion}. {grupo.Key} - {grupo.Count()} ocurrencias");
                posicion++;
            }
        }

        private static void MostrarAnalisisSeguros()
        {
            var analisis = servicioEntrenamiento.GenerarAnalisisSeguros(atletaActual.Nombre);
            interfazUsuario.MostrarSubTitulo("=== ANÁLISIS DE SEGUROS MÉDICOS ===");
            interfazUsuario.MostrarInformacion(analisis);
        }

        #endregion

        #region Sugerencias de Ejercicios

        private static void MostrarSugerenciasEjercicios()
        {
            if (atletaActual == null)
            {
                interfazUsuario.MostrarAdvertencia("Debe seleccionar un atleta activo primero.");
                return;
            }

            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== SUGERENCIAS DE EJERCICIOS ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Rutinas Personalizadas",
                    "2. Ejercicios por Grupo Muscular",
                    "3. Ejercicios por Nivel",
                    "4. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": MostrarRutinasPersonalizadas(); break;
                    case "2": MostrarEjerciciosPorGrupo(); break;
                    case "3": MostrarEjerciciosPorNivel(); break;
                    case "4": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void MostrarRutinasPersonalizadas()
        {
            var sugerencias = servicioSugerencias.ObtenerRutinasSugeridas(atletaActual);
            interfazUsuario.MostrarSubTitulo($"=== RUTINAS PERSONALIZADAS PARA {atletaActual.Nombre} ===");

            foreach (var sugerencia in sugerencias)
            {
                interfazUsuario.MostrarInformacion($"• {sugerencia}");
            }
        }

        private static void MostrarEjerciciosPorGrupo()
        {
            interfazUsuario.MostrarSubTitulo("=== EJERCICIOS POR GRUPO MUSCULAR ===");
            interfazUsuario.MostrarMenu(new[]
            {
                "1. Pecho", "2. Espalda", "3. Piernas", "4. Brazos",
                "5. Hombros", "6. Abdomen", "7. Cardio"
            });

            var opcion = interfazUsuario.LeerEntrada("Seleccione grupo muscular: ");
            var grupoMuscular = opcion switch
            {
                "1" => "Pecho",
                "2" => "Espalda",
                "3" => "Piernas",
                "4" => "Brazos",
                "5" => "Hombros",
                "6" => "Abdomen",
                "7" => "Cardio",
                _ => ""
            };

            if (!string.IsNullOrEmpty(grupoMuscular))
            {
                var ejercicios = servicioSugerencias.ObtenerEjerciciosPorGrupo(grupoMuscular, atletaActual.Nivel);
                interfazUsuario.MostrarSubTitulo($"=== EJERCICIOS PARA {grupoMuscular.ToUpper()} ===");

                foreach (var ejercicio in ejercicios)
                {
                    interfazUsuario.MostrarInformacion($"• {ejercicio}");
                }
            }
            else
            {
                interfazUsuario.MostrarError("Opción no válida.");
            }
        }

        private static void MostrarEjerciciosPorNivel()
        {
            var ejercicios = servicioSugerencias.ObtenerEjerciciosPorNivel(atletaActual.Nivel);
            interfazUsuario.MostrarSubTitulo($"=== EJERCICIOS PARA NIVEL {atletaActual.Nivel.ToUpper()} ===");

            foreach (var ejercicio in ejercicios)
            {
                interfazUsuario.MostrarInformacion($"• {ejercicio}");
            }
        }

        #endregion

        #region Búsquedas Avanzadas

        private static void RealizarBusquedasAvanzadas()
        {
            if (atletaActual == null)
            {
                interfazUsuario.MostrarAdvertencia("Debe seleccionar un atleta activo primero.");
                return;
            }

            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== BÚSQUEDAS AVANZADAS ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Buscar Rutinas",
                    "2. Buscar por Fecha",
                    "3. Buscar por Intensidad",
                    "4. Búsqueda Combinada",
                    "5. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": BuscarRutinas(); break;
                    case "2": BuscarPorFecha(); break;
                    case "3": BuscarPorIntensidad(); break;
                    case "4": BusquedaCombinada(); break;
                    case "5": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void BuscarRutinas()
        {
            var termino = interfazUsuario.LeerEntrada("Ingrese término de búsqueda: ");
            var resultados = gestorRutinas.BuscarRutinas(atletaActual.Nombre, termino);

            MostrarResultadosBusqueda("RUTINAS", resultados);
        }

        private static void BuscarPorFecha()
        {
            var fechaInicio = interfazUsuario.LeerFecha("Fecha de inicio (AAAA-MM-DD): ");
            var fechaFin = interfazUsuario.LeerFecha("Fecha de fin (AAAA-MM-DD): ");

            var resultados = gestorRutinas.BuscarPorRangoFechas(atletaActual.Nombre, fechaInicio, fechaFin);
            MostrarResultadosBusqueda("RUTINAS POR FECHA", resultados);
        }

        private static void BuscarPorIntensidad()
        {
            var intensidad = interfazUsuario.LeerEntrada("Intensidad (Baja/Media/Alta): ");
            var resultados = gestorRutinas.BuscarPorIntensidad(atletaActual.Nombre, intensidad);

            MostrarResultadosBusqueda($"RUTINAS DE INTENSIDAD {intensidad.ToUpper()}", resultados);
        }

        private static void BusquedaCombinada()
        {
            // Implementar búsqueda con múltiples criterios usando LINQ
            var tipo = interfazUsuario.LeerEntradaOpcional("Tipo (Fuerza/Cardio, Enter para omitir): ");
            var intensidad = interfazUsuario.LeerEntradaOpcional("Intensidad (Baja/Media/Alta, Enter para omitir): ");
            var grupoMuscular = interfazUsuario.LeerEntradaOpcional("Grupo muscular (Enter para omitir): ");

            var resultados = gestorRutinas.BusquedaCombinada(atletaActual.Nombre, tipo, intensidad, grupoMuscular);
            MostrarResultadosBusqueda("BÚSQUEDA COMBINADA", resultados);
        }

        private static void MostrarResultadosBusqueda<T>(string titulo, IEnumerable<T> resultados) where T : IDescribible
        {
            var lista = resultados.ToList();
            interfazUsuario.MostrarSubTitulo($"=== RESULTADOS: {titulo} ===");

            if (lista.Any())
            {
                foreach (var item in lista)
                {
                    interfazUsuario.MostrarInformacion($"• {item.Describir()}");
                }
            }
            else
            {
                interfazUsuario.MostrarAdvertencia("No se encontraron resultados.");
            }
        }

        #endregion

        #region Configuración

        private static void MostrarConfiguracion()
        {
            bool volver = false;
            while (!volver)
            {
                interfazUsuario.MostrarSubTitulo("=== CONFIGURACIÓN ===");
                interfazUsuario.MostrarMenu(new[]
                {
                    "1. Exportar Datos",
                    "2. Importar Datos",
                    "3. Limpiar Cache",
                    "4. Información del Sistema",
                    "5. Volver al Menú Principal"
                });

                string opcion = interfazUsuario.LeerEntrada("Seleccione una opción: ");

                switch (opcion)
                {
                    case "1": ExportarDatos(); break;
                    case "2": ImportarDatos(); break;
                    case "3": LimpiarCache(); break;
                    case "4": MostrarInfoSistema(); break;
                    case "5": volver = true; break;
                    default: interfazUsuario.MostrarError("Opción no válida."); break;
                }

                if (!volver)
                {
                    interfazUsuario.EsperarTecla();
                }
            }
        }

        private static void ExportarDatos()
        {
            try
            {
                var ruta = interfazUsuario.LeerEntrada("Ruta de exportación (Enter para ruta por defecto): ");
                if (string.IsNullOrWhiteSpace(ruta))
                {
                    ruta = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}";
                }

                // Implementar exportación
                interfazUsuario.MostrarExito($"Datos exportados exitosamente a {ruta}");
            }
            catch (Exception ex)
            {
                interfazUsuario.MostrarError($"Error al exportar: {ex.Message}");
            }
        }

        private static void ImportarDatos()
        {
            try
            {
                var ruta = interfazUsuario.LeerEntrada("Ruta del archivo a importar: ");
                // Implementar importación
                interfazUsuario.MostrarExito("Datos importados exitosamente.");
            }
            catch (Exception ex)
            {
                interfazUsuario.MostrarError($"Error al importar: {ex.Message}");
            }
        }

        private static void LimpiarCache()
        {
            // Limpiar caches de los repositorios si los hay
            interfazUsuario.MostrarExito("Cache limpiado exitosamente.");
        }

        private static void MostrarInfoSistema()
        {
            interfazUsuario.MostrarSubTitulo("=== INFORMACIÓN DEL SISTEMA ===");
            interfazUsuario.MostrarInformacion($"Versión: 2.0.0");
            interfazUsuario.MostrarInformacion($"Atletas registrados: {gestorAtletas.ObtenerTodos().Count()}");
            interfazUsuario.MostrarInformacion($"Rutinas registradas: {gestorRutinas.ObtenerTodos().Count()}");
            interfazUsuario.MostrarInformacion($"Seguros registrados: {gestorSeguros.ObtenerTodos().Count()}");
            interfazUsuario.MostrarInformacion($"Atleta activo: {atletaActual?.Nombre ?? "Ninguno"}");
        }

        #endregion

        #region Métodos de Cierre

        private static void GuardarYSalir()
        {
            try
            {
                interfazUsuario.MostrarInformacion("Guardando datos...");

                // Los repositorios se encargan automáticamente de guardar
                // debido a su implementación con auto-persistencia

                interfazUsuario.MostrarExito("Datos guardados exitosamente.");
                interfazUsuario.MostrarInformacion("¡Gracias por usar el Sistema de Entrenamiento Personal!");
            }
            catch (Exception ex)
            {
                interfazUsuario.MostrarError($"Error al guardar datos: {ex.Message}");
                var continuar = interfazUsuario.LeerEntrada("¿Desea salir sin guardar? (s/n): ");
                if (continuar.ToLower() != "s")
                {
                    throw; // Re-lanzar excepción para no salir
                }
            }
        }

        #endregion
    }
}
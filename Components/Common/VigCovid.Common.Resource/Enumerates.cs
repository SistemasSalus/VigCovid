namespace VigCovid.Common.Resource
{
    public class Enums
    {
        public enum TipoUsuario
        {
            MedicoVigilancia = 1,
            MedicoAdministrador = 2,
            RecursosHumanos = 3,
            AdministradorGeneral = 4,
            Enfermera = 5
        }

        public enum ModoIngreso
        {
            Sintomatico = 1,
            Sospechoso = 2,
            ContactoDirecto = 3,
            AsintomaticoPositivo = 4,
            Reingreso = 5,
            CovidConfirmadoSintomatico = 6
        }

        public enum ViaIngreso
        {
            Tamizaje = 1,
            Garita = 2,
            AreaTrabajo = 3,
            Domicilio = 4
        }

        public enum CodigoEmpresa
        {
            SalusLaboris = 1,
            Backus = 2,
            Ambev = 3
        }

        public enum ResultadoCovid19
        {
            Negativo = 0,
            Novalido = 1,
            IgMPositivo = 2,
            IgGPositivo = 3,
            IgMeIgGpositivo = 4,
            Noserealizo = 5,
            Positivo = 6,
        }

        public enum TipoExamenCovid
        {

            //Modificado por Saul Ramos Vega   -  20210712
            Pr = 1,
            Molecular = 2,
            Antigenos = 3

            // Pr = 1,
            //Molecular = 000,
            //Antigenos = 3


        }

        public enum EstadoClinico
        {
            alta = 1
        }

        public enum TipoEstado
        {
            Cuarentena = 1,
            Hospitalizado = 2,
            Fallecido = 3,
            AislamientoLeve = 4,
            AltaEpidemiologica = 5,
            AislamientoModerado = 6,
            AislamientoSevero = 7,
            AislamientoCasoAsintomatico = 8,
            AislamientoPostHospitalitario = 9,
        }
    }
}
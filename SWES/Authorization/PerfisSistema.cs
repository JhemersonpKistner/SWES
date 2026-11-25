namespace SWES.Authorization
{
    public static class PerfisSistema
    {
        public const string Administrador = "Administrador";
        public const string Aluno = "Aluno";
        public const string Professor = "Professor";
        public const string Supervisor = "Supervisor";
        public const string Coordenador = "Coordenador";

        public const string Todos = Administrador + "," + Aluno + "," +
            Professor + "," + Supervisor + "," + Coordenador;

        public const string Gestores = Administrador + "," + Coordenador;

        public static string ParaTipoUsuario(string tipo) => tipo switch
        {
            "Aluno" => Aluno,
            "Professor" => Professor,
            "Supervisor" => Supervisor,
            "Coordenador" => Coordenador,
            _ => throw new ArgumentException("Perfil de usuário inválido.", nameof(tipo))
        };
    }
}

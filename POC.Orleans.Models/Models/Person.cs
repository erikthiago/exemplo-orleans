using System;

namespace POC.Orleans.Models.Models
{
    /// <summary>
    /// Classe que representa a tabela no banco de dados com seus dados
    /// Implementei a intereface IEquatable para eu possa usar o Equals no teste e verificar se o que salvei no banco é igual ao que pesquisei com os mesmos dados
    /// </summary>
    public class Person : IEquatable<Person>
    {
        /// <summary>
        /// Nome da pessoa
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Data de nascimento da pessoa
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Endereço da pessoa
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// CPF da pessoa
        /// </summary>
        public string CPF { get; set; }

        /// <summary>
        /// Usado para verificar os campos e não outros dados
        /// </summary>
        /// <param name="other">Objeto a ser comparado</param>
        /// <returns>Retorna true para igual e false para não igual</returns>
        public bool Equals(Person? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetHashCode() != other.GetHashCode()) return false;

            return Name == other.Name && Address == other.Address;
        }

        /// <summary>
        /// Método que sobrescreve o equals já existente para usar o método acima de verificação de campos
        /// </summary>
        /// <param name="obj">Objeto a ser comparado</param>
        /// <returns>Retorna true para igual e false para não igual</returns>
        public override bool Equals(object? obj) => Equals(obj as Person);
        /// <summary>
        /// Outra forma de gerar o HasCode da classe
        /// </summary>
        /// <returns>Retorna o hascode da classe</returns>
        public override int GetHashCode() => HashCode.Combine(CPF, BirthDate);
    }
}

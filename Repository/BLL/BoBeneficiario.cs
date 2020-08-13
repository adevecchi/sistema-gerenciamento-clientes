using System.Collections.Generic;

namespace Repository.BLL
{
    public class BoBeneficiario
    {
        /// <summary>
        /// Inclui um novo beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        public long Incluir(DML.Beneficiario beneficiario)
        {
            DAL.DaoBeneficiario benefi = new DAL.DaoBeneficiario();
            return benefi.Incluir(beneficiario);
        }

        /// <summary>
        /// Consulta os beneficiarios do cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public List<DML.Beneficiario> Consultar(long id)
        {
            DAL.DaoBeneficiario benefi = new DAL.DaoBeneficiario();
            return benefi.Consultar(id);
        }

        /// <summary>
        /// Excluir os beneficiarios do cliente pelo id
        /// </summary>
        /// <param name="IdCliente">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long IdCliente)
        {
            DAL.DaoBeneficiario benefi = new DAL.DaoBeneficiario();
            benefi.Excluir(IdCliente);
        }

        /// <summary>
        /// VerificaExistencia
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public bool VerificarExistencia(string CPF, long IdCliente)
        {
            DAL.DaoBeneficiario benefi = new DAL.DaoBeneficiario();
            return benefi.VerificarExistencia(CPF, IdCliente);
        }
    }
}

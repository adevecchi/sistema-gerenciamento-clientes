using System.Collections.Generic;
using System.Data;

namespace Repository.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Beneficiario
    /// </summary>
    internal class DaoBeneficiario : AcessoDados
    {
        /// <summary>
        /// Inclui um novo beneficiario
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        internal long Incluir(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", beneficiario.IdCliente));

            DataSet ds = base.Consultar("FI_SP_IncClienteBeneficiarios", parametros);
            long ret = 0;

            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);

            return ret;
        }

        /// <summary>
        /// Consulta beneficiarios pelo id do cliente
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiario</param>
        internal List<DML.Beneficiario> Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsClienteBeneficiarios", parametros);
            List<DML.Beneficiario> benefi = Converter(ds);

            return benefi;
        }

        /// <summary>
        /// Excluir Beneficiarios
        /// </summary>
        /// <param name="IdCliente">Id do cliente</param>
        internal void Excluir(long IdCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", IdCliente));

            base.Executar("FI_SP_DelClienteBeneficiarios", parametros);
        }

        internal bool VerificarExistencia(string CPF, long IdCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", IdCliente));

            DataSet ds = base.Consultar("FI_SP_VerificaClienteBeneficiarios", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        private List<DML.Beneficiario> Converter(DataSet ds)
        {
            List<DML.Beneficiario> lista = new List<DML.Beneficiario>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Beneficiario benefi = new DML.Beneficiario();
                    benefi.Id = row.Field<long>("Id");
                    benefi.CPF = row.Field<string>("CPF");
                    benefi.Nome = row.Field<string>("Nome");
                    benefi.IdCliente = row.Field<long>("IdCliente");
                    
                    lista.Add(benefi);
                }
            }

            return lista;
        }
    }
}

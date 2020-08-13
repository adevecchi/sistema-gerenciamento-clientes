using WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Repository.BLL;
using Repository.DML;

namespace WebApp.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (bo.VerificarExistencia(model.CPF))
                    return Json(new { Result = false, Code = 1, Message = "CPF informado para novo Cliente já existe!" });

                model.Id = bo.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                if (model.Beneficiarios != null)
                {
                    BoBeneficiario boBenefi = new BoBeneficiario();

                    string strCpf = "";
                    
                    dynamic dynBenefi = Newtonsoft.Json.JsonConvert.DeserializeObject(model.Beneficiarios);

                    foreach (var objBenefi in dynBenefi)
                    {
                        if (boBenefi.VerificarExistencia(objBenefi.cpf.ToString(), model.Id))
                        {
                            strCpf += objBenefi.cpf.ToString() + "<br>";
                        }
                        else
                        {
                            boBenefi.Incluir(new Beneficiario()
                            {
                                CPF = objBenefi.cpf.ToString(),
                                Nome = objBenefi.nome.ToString(),
                                IdCliente = model.Id
                            });
                        }
                    }
                    
                    if (strCpf.Length > 0)
                        return Json(new { Result = false, Code = 2, Message = "Cadastro do Cliente efetuado com sucesso.<br><br>Porém, Beneficiários com os CPF abaixo já exitem:<br><br>" + strCpf });
                }
                
                return Json(new { Result = true, Message = "Cadastro efetuado com sucesso" });
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                BoBeneficiario boBenefi = new BoBeneficiario();

                if (model.Beneficiarios != null)
                {
                    string strCpf = "";

                    dynamic dynBenefi = Newtonsoft.Json.JsonConvert.DeserializeObject(model.Beneficiarios);

                    boBenefi.Excluir(model.Id);

                    foreach (var objBenefi in dynBenefi)
                    {
                        if (boBenefi.VerificarExistencia(objBenefi.cpf.ToString(), model.Id))
                        {
                            strCpf += objBenefi.cpf.ToString() + "<br>";
                        }
                        else
                        { 
                            boBenefi.Incluir(new Beneficiario()
                            {
                                CPF = objBenefi.cpf.ToString(),
                                Nome = objBenefi.nome.ToString(),
                                IdCliente = model.Id
                            });
                        }
                    }
                    
                    if (strCpf.Length > 0)
                        return Json(new { Result = false, Message = "Cadastro do Cliente alterado com sucesso.<br><br>Porém, Beneficiários com os CPF abaixo já exitem:<br><br>" + strCpf });
                }
                else
                {
                    boBenefi.Excluir(model.Id);
                }
                
                return Json(new { Result = true, Message = "Cadastro alterado com sucesso" });
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            
            BoBeneficiario boBenefi = new BoBeneficiario();

            ClienteBeneficiariosModel model = new ClienteBeneficiariosModel();

            if (cliente != null)
            {
                model.Cliente = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };

                model.Beneficiarios = new ResultSetBeneficiariosModel();
                model.Beneficiarios.ResultSet = boBenefi.Consultar(model.Cliente.Id);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);
                
                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
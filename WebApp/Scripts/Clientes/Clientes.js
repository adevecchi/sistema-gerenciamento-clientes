
$(document).ready(function () {

    var SPMaskBehavior = function (val) {
            return val.replace(/\D/g, '').length === 11 ? '(00) 00000-0000' : '(00) 0000-00009';
        },
        spOptions = {
            onKeyPress: function (val, e, field, options) {
                field.mask(SPMaskBehavior.apply({}, arguments), options);
            }
        };

    $('#formCadastro #CPF').mask('000.000.000-00');
    $('#formCadastro #CEP').mask('00000-000');
    $('#formCadastro #Telefone').mask(SPMaskBehavior, spOptions);

    $('#formBeneficiarios #CPF').mask('000.000.000-00');

    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        if (!verificarCPF($(this).find("#CPF").val())) {
            ModalDialog("CPF Inválido", "Favor digitar um CPF válido.");
            $(this).find("#CPF").val("");
            return;
        }

        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "CPF": $(this).find("#CPF").val(),
                "Beneficiarios": ObtemBenificiarios()
            },
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    if (r.Result) {
                        ModalDialog("Sucesso!", r.Message)
                        $("#formCadastro")[0].reset();
                        $("#formBeneficiarios table tbody").empty();
                    } else {
                        ModalDialog("Aviso!", r.Message);
                        if (r.Code == 2) {
                            $("#formCadastro")[0].reset();
                            $("#formBeneficiarios table tbody").empty();
                        }
                    }
                }
        });
    });

    $("#btn-beneficiarios").click(function (evt) {
        $("#formBeneficiarios").modal("show");
    });

    $("#formBeneficiarios #incluir").click(function (evt) {
        var $cpf = $("#formBeneficiarios #CPF"),
            $nome = $("#formBeneficiarios #Nome");

        if (!verificarCPF($cpf.val())) {
            ModalDialog("CPF Inválido", "Favor digitar um \"CPF\" válido.");
            $cpf.val("");
            return;
        }

        if ($nome.val().length < 5) {
            ModalDialog("Nome Inválido", "Favor digitar um \"Nome\" válido.");
            $nome.val("");
            return;
        }

        IncluirBeneficiarios($cpf.val(), $nome.val());

        $cpf.val("");
        $nome.val("");
    });

    $("#formBeneficiarios").on("click", ".btn-excluir", function (evt) {
        $(this).parent().parent().remove();
    });

    $("#formBeneficiarios").on("click", ".btn-alterar", function (evt) {
        var $tr = $(this).parent().parent();

        $("#formBeneficiarios #CPF").val($tr.children("td:nth-child(1)").text());
        $("#formBeneficiarios #Nome").val($tr.children("td:nth-child(2)").text());

        $tr.remove();
    });

});

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}

function IncluirBeneficiarios(cpf, nome) {
    var rowTable = '';

    rowTable += '<tr>';
    rowTable += '    <td>' + cpf + '</td>';
    rowTable += '    <td>' + nome + '</td>';
    rowTable += '   <td>';
    rowTable += '       <button type="button" class="btn btn-sm btn-primary btn-alterar">Alterar</button>';
    rowTable += '       &nbsp; &nbsp;';
    rowTable += '       <button type="button" class="btn btn-sm btn-primary btn-excluir">Excluir</button>';
    rowTable += '   </td>';
    rowTable += '</tr>';

    $("#formBeneficiarios table tbody").append(rowTable);
}

function ObtemBenificiarios() {
    var $tr,
        beneficiarios = [];

    $("#formBeneficiarios table tbody tr").each(function () {
        $tr = $(this);

        beneficiarios.push({
            cpf: $tr.children("td:nth-child(1)").text(),
            nome: $tr.children("td:nth-child(2)").text()
        });

    });

    return JSON.stringify(beneficiarios);
}

function verificarCPF(cpf) {
    var add,
        rev;

    // Remove os pontos/traço da expressão regular, caso exista
    cpf = cpf.replace(/[^\d]+/g, '');
    if (cpf == '') return false;

    // Elimina CPFs invalidos conhecidos    
    if (cpf.length != 11 ||
        cpf == "00000000000" ||
        cpf == "11111111111" ||
        cpf == "22222222222" ||
        cpf == "33333333333" ||
        cpf == "44444444444" ||
        cpf == "55555555555" ||
        cpf == "66666666666" ||
        cpf == "77777777777" ||
        cpf == "88888888888" ||
        cpf == "99999999999")
        return false;

    // Valida 1o digito 
    add = 0;

    for (var i = 0; i < 9; i++) {
        add += parseInt(cpf.charAt(i)) * (10 - i);
    }

    rev = 11 - (add % 11);

    if (rev == 10 || rev == 11) {
        rev = 0;
    }

    if (rev != parseInt(cpf.charAt(9))) {
        return false;
    }

    // Valida 2o digito 
    add = 0;

    for (var i = 0; i < 10; i++) {
        add += parseInt(cpf.charAt(i)) * (11 - i);
    }

    rev = 11 - (add % 11);

    if (rev == 10 || rev == 11) {
        rev = 0;
    }

    if (rev != parseInt(cpf.charAt(10))) {
        return false;
    }

    return true;
}

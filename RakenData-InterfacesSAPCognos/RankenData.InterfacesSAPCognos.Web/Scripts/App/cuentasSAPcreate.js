
    $(document).ready(function (e) {       
        $('#divEsOpen').hide();
        $('#divCuentaCargo').hide();
        $('#divCuentaAbono').hide();

        $('#TipoCuentaSAP').change(function () {
            if ($('#TipoCuentaSAP').val() == 1) {
                $('#divEsOpen').hide();
                $('#divCuentaCargo').hide();
                $('#divCuentaAbono').hide();
            }
            else {
                $('#divEsOpen').show();
            }            
        });
        $('#EsOpen').change(function () {
            if ($('#EsOpen').val() == "true") {
                $('#divCuentaCargo').show();
                $('#divCuentaAbono').show();
            }
            else {
                $('#divCuentaCargo').hide();
                $('#divCuentaAbono').hide();
            }
        });
    });

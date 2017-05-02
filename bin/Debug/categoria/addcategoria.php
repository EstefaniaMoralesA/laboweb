<?php
    include_once("../header.php");
?>
<div class='col-sm-12' style='margin-top: 70px; text-align: center;'>
<h2 style='margin-top: 0px; font-weight: bold; font-size: 35px;color: #E42B22;'>Añadir categoria</h2>
</div>
<div class='col-lg-10 col-lg-offset-1'>
<form action='' method='post'>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='nombre' placeholder='Nombre' type='text' value=''>
</div>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='descripcion' placeholder='descripcion' type='text' value=''>
</div>
<div class='form-group' style='width: 100%; margin-bottom: 0px;'>
<div class='col-sm-6 col-sm-offset-3' style='text-align: center;'>
<input type='submit' value='Guardar' class='btn btn-primary btn-lg' style='padding: 7px 0px !important; width: 100%; text-align: center;'>
</div>
<div class='col-sm-6 col-sm-offset-3' style='text-align: center; margin-top:10px;'>
<a class='btn btn-default btn-lg' href='categoria.php' style='padding: 7px 0px !important; width: 100%; text-align: center;'>Cancelar</a>
</div>
</div>
</form>
</div>
<?php
    include_once("../footer.php");
?>

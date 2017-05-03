<?php
    include_once("../header.php");
$result = NULL;
if(empty($_GET['pk'])){
echo 'ERROR no se ha encontrado un registro para editar'; exit();}
$pk = $_GET['pk'];
function getValues($db, $pk){
$query = "SELECT `nombre`,`descripcion` FROM categoria WHERE id = " . '' . "1QQ";
return $db->qarray($query, array($pk));
}
if(!empty($_POST)){
$error = 0;
if(empty($_POST['nombre']))
{
$error = 1;
}
else{
$nombre = $_POST['nombre'];
}
if(empty($_POST['descripcion']))
{
$error = 1;
}
else{
$descripcion = $_POST['descripcion'];
}
if($error == 1){
$result = getValues($db, $pk);
echo '<div class="alert alert-danger">ERROR se deben de llenar todos los campos de la forma.</div>';}
else{
$query = "UPDATE categoria SET nombre = '1QQ',descripcion = '1QQ' WHERE id = " . '' . "1QQ";
$result = $db->squery_rows($query, array($nombre,$descripcion, $pk));
if($result == 1){
header('Location: categoria.php');
}
}
}
else{
$result = getValues($db, $pk);
}
?>
<div class='col-sm-12' style='margin-top: 70px; text-align: center;'>
<h2 style='margin-top: 0px; font-weight: bold; font-size: 35px;color: #E42B22;'>Editar categoria</h2>
</div>
<div class='col-lg-10 col-lg-offset-1'>
<form action='editcategoria.php?pk=<?php echo $pk; ?>' method='post'>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='nombre' placeholder='Nombre' type='text' value='<?php echo $result['nombre'] ?>'>
</div>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='descripcion' placeholder='descripcion' type='text' value='<?php echo $result['descripcion'] ?>'>
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

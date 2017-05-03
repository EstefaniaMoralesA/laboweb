<?php
    include_once("../header.php");
$result = NULL;
if(empty($_GET['pk'])){
echo 'ERROR no se ha encontrado un registro para editar'; exit();}
$pk = $_GET['pk'];
function getValues($db, $pk){
$query = "SELECT `nombre`,`numeroCliente`,`idCategoria`,`genero` FROM cliente WHERE id = " . '' . "1QQ";
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
if(empty($_POST['numeroCliente']))
{
$error = 1;
}
else{
$numeroCliente = $_POST['numeroCliente'];
}
if(empty($_POST['idCategoria']))
{
$error = 1;
}
else{
$idCategoria = $_POST['idCategoria'];
}
if(!isset($_POST['genero']) || $_POST['genero'] == '')
{
$error = 1;
}
else{
$genero = $_POST['genero'];
}
if($error == 1){
$result = getValues($db, $pk);
echo '<div class="alert alert-danger">ERROR se deben de llenar todos los campos de la forma.</div>';}
else{
$query = "UPDATE cliente SET nombre = '1QQ',numeroCliente = '1QQ',idCategoria = '1QQ',genero = '1QQ' WHERE id = " . '' . "1QQ";
$result = $db->squery_rows($query, array($nombre,$numeroCliente,$idCategoria,$genero, $pk));
if($result == 1){
header('Location: cliente.php');
}
}
}
else{
$result = getValues($db, $pk);
}
?>
<div class='col-sm-12' style='margin-top: 70px; text-align: center;'>
<h2 style='margin-top: 0px; font-weight: bold; font-size: 35px;color: #E42B22;'>Editar cliente</h2>
</div>
<div class='col-lg-10 col-lg-offset-1'>
<form action='editcliente.php?pk=<?php echo $pk; ?>' method='post'>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='nombre' placeholder='nombre' type='text' value='<?php echo $result['nombre'] ?>'>
</div>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='numeroCliente' placeholder='numeroCliente' type='text' value='<?php echo $result['numeroCliente'] ?>'>
</div>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='idCategoria' placeholder='Categoria' type='number' value='<?php echo $result['idCategoria'] ?>'>
</div>
<div class='form-group'>
<select name='genero'>

<?php if($result['genero'] == 0) : ?>
<option selected value='<?php echo $result['genero']; ?>'>masculino</option>
<?php else : ?>
<option value='0'>masculino</option>
<?php endif; ?>
<?php if($result['genero'] == 1) : ?>
<option selected value='<?php echo $result['genero']; ?>'>femenino</option>
<?php else : ?>
<option value='1'>femenino</option>
<?php endif; ?>
</select>
</div>
<div class='form-group' style='width: 100%; margin-bottom: 0px;'>
<div class='col-sm-6 col-sm-offset-3' style='text-align: center;'>
<input type='submit' value='Guardar' class='btn btn-primary btn-lg' style='padding: 7px 0px !important; width: 100%; text-align: center;'>
</div>
<div class='col-sm-6 col-sm-offset-3' style='text-align: center; margin-top:10px;'>
<a class='btn btn-default btn-lg' href='cliente.php' style='padding: 7px 0px !important; width: 100%; text-align: center;'>Cancelar</a>
</div>
</div>
</form>
</div>
<?php
    include_once("../footer.php");
?>

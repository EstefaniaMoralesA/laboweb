<?php
    include_once("../header.php");
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
if(!isset($_POST['idCategoria']) || $_POST['idCategoria'] == '')
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
echo '<div class="alert alert-danger">ERROR se deben de llenar todos los campos de la forma.</div>';}
else{
$query = "INSERT INTO cliente(`id`,`nombre`,`numeroCliente`,`idCategoria`,`genero`) VALUES ('1QQ','1QQ','1QQ','1QQ','1QQ')";
$result = $db->squery_rows($query, array(NULL,$nombre,$numeroCliente,$idCategoria,$genero));
if($result == 1){
header('Location: cliente.php');
}
}
}
?>
<div class='col-sm-12' style='margin-top: 70px; text-align: center;'>
<h2 style='margin-top: 0px; font-weight: bold; font-size: 35px;color: #E42B22;'>Añadir cliente</h2>
</div>
<div class='col-lg-10 col-lg-offset-1'>
<form action='addcliente.php' method='post'>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='nombre' placeholder='nombre' type='text' value=''>
</div>
<div class='form-group'>
<input class='form-control text-box single-line input-validation-error' data-val='true' data-val-required='Campo requerido' name='numeroCliente' placeholder='numeroCliente' type='text' value=''>
</div>
<div class='form-group'>
<?php $fkquery = 'SELECT * FROM categoria';
$fkresult = $db->squery_rows($fkquery, array()); ?>
<select name='idCategoria'>
<option selected disabled value=''>Selecciona Categoria</option>
<?php while($data = mysqli_fetch_assoc($fkresult)) : ?>
<option value="<?php echo $data['id'];?>"><?php echo $data['descripcion']; ?></option>
<?php endwhile; ?>
</select>
</div>
<div class='form-group'>
<select name='genero'>
<option selected disabled value=''>Selecciona genero</option>
<option value='0'>masculino</option>
<option value='1'>femenino</option>
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

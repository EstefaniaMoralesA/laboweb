<?php
    include_once("../header.php");
if(!empty( $_GET['pk'])){
$query = "DELETE FROM cliente WHERE id = " . '' . "1QQ";
$result = $db->squery_rows($query, array($_GET['pk']));
if($result == 1){
header('Location: cliente.php');
}
else{
echo 'ERROR al eliminar registro. Vuelva a intentarlo.';
}
}
?>
<div class='row' style='margin-top:70px;'>
<ul class='nav nav-tabs'>
<li><a href='../categoria/categoria.php'>categoria</a></li>
<li class='active'><a href='#'>cliente</a></li>
</ul>
</div>
<a href='addcliente.php' class='btn btn-success' style='margin-top: 40px; padding-left: 39px; padding-right: 39px;'>Añadir cliente</a>
<div class='row' style='margin-top:30px;'>
<div class='table-responsive'>
<table class='table'>
<thead>
<tr>
<th>id</th>
<th>nombre</th>
<th>numeroCliente</th>
<th>Categoria</th>
<th>genero</th>
<th></th>
<th></th>
</tr>
</thead>
<tbody>
<?php
$resultSet = $db->squery_rows('SELECT id,nombre,numeroCliente,idCategoria,genero FROM cliente ', array());
while($data = mysqli_fetch_assoc($resultSet)){
echo '<tr>';
echo '<td>'.$data['id'].'</td>';
echo '<td>'.$data['nombre'].'</td>';
echo '<td>'.$data['numeroCliente'].'</td>';
$fkquery = "SELECT nombre FROM categoria WHERE id = '1QQ'";
$fkresult = $db->qarray($fkquery, array($data['idCategoria']));
echo '<td>'.$fkresult['nombre'].'</td>';
$array = array("0" => "masculino","1" => "femenino");
echo '<td>'.$array[$data['genero']].'</td>';
echo '<td><a href="editcliente.php?pk='.$data['id'].'" class="btn btn-primary" style="padding-left: 46px; padding-right: 46px;">Editar</a></td>';
echo '<td><a href="cliente.php?pk='.$data['id'].'"  onclick="return confirm(\'Seguro quieres borrar este registro?\');" class="btn btn-danger" style="padding-left: 39px; padding-right: 39px;">Eliminar</a></td>';
echo '</tr>';}
?>
</tbody>
</table>
</div>
</div>
<?php
    include_once("../footer.php");
?>

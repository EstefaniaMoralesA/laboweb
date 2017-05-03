<?php
    include_once("../header.php");
if(!empty( $_GET['pk'])){
$query = "DELETE FROM categoria WHERE id = " . '' . "1QQ";
$result = $db->squery_rows($query, array($_GET['pk']));
if($result == 1){
header('Location: categoria.php');
}
else{
echo 'ERROR al eliminar registro. Vuelva a intentarlo.';
}
}
?>
<div class='row' style='margin-top:70px;'>
<ul class='nav nav-tabs'>
<li class='active'><a href='#'>categoria</a></li>
<li><a href='../cliente/cliente.php'>cliente</a></li>
</ul>
</div>
<a href='addcategoria.php' class='btn btn-success' style='margin-top: 40px; padding-left: 39px; padding-right: 39px;'>Añadir categoria</a>
<div class='row' style='margin-top:30px;'>
<div class='table-responsive'>
<table class='table'>
<thead>
<tr>
<th>id</th>
<th>Nombre</th>
<th>descripcion</th>
<th></th>
</tr>
</thead>
<tbody>
<?php
$resultSet = $db->squery_rows('SELECT id,nombre,descripcion FROM categoria ', array());
while($data = mysqli_fetch_assoc($resultSet)){
echo '<tr>';
echo '<td>'.$data['id'].'</td>';
echo '<td>'.$data['nombre'].'</td>';
echo '<td>'.$data['descripcion'].'</td>';
echo '<td><a href="editcategoria.php?pk='.$data['id'].'" class="btn btn-primary" style="padding-left: 46px; padding-right: 46px;">Editar</a></td>';
echo '</tr>';}
?>
</tbody>
</table>
</div>
</div>
<?php
    include_once("../footer.php");
?>

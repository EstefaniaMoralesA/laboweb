<?php
    include_once("../header.php");
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
<th>idCategoria</th>
<th>genero</th>
<th></th>
<th></th>
</tr>
</thead>
<tbody>
<?php
$resultSet = $db->squery_rows('SELECT id,nombre,numeroCliente,idCategoria,genero FROM cliente WHERE id NOT IN (1,4)', array());
while($data = mysqli_fetch_row($resultSet)){
echo '<tr>';
foreach($data as $col){
echo '<td>'.$col.'</td>';}
echo '<td><a class="btn btn-primary" style="padding-left: 46px; padding-right: 46px;">Editar</a></td>';
echo '<td><a class="btn btn-danger" style="padding-left: 39px; padding-right: 39px;">Eliminar</a></td>';
echo '</tr>';}
?>
</tbody>
</table>
</div>
</div>
<?php
    include_once("../footer.php");
?>

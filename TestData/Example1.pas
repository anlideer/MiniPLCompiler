begin
var i, j : integer;
 read (i, j);
 while i <> j do
 if i > j then i := i - j;
 else j := j - i;
writeln (i);
end.
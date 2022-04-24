program Test1;
begin
var i, j : integer;
var r : real;
 r := 12.56e-2;
 read (i, j);
 while i <> j do
 if i > j then i := i - j;
 else j := j - i;
writeln (i);
end.
program MutualRecursion;
function F (n : integer) : integer;
begin
if n = 0 then
return 1;
else
return n - M (F (n-1));
end;
function M (n : integer) : integer;
begin
if n = 0 then return 0;
else return n - F (M (n-1));
end;
begin
var i : integer;
i := 0;
while i <= 19 do writeln (F (i));
i := 0;
while i <= 19 do writeln (M (i));
end. 
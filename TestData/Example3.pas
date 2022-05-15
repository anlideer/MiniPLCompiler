program SwapAndSumThem;
function Sum (data : array [] of integer) : integer;
begin
var i, sum : integer;
i := 0; sum := 0;
while i < data.size do begin
sum := sum + data [i]; i := i + 1;
end;
return sum;
end;

procedure Swap (var i : integer, var j : integer);
begin
var tmp : integer;
tmp := i; i := j; j := tmp;
return;
end;

begin
var A : array [2] of integer;
read (A [0], A [1]);
Swap (A [0], A [1]);
writeln (A [0], A [1]);
writeln ("Sum is ", sum (A));
end.

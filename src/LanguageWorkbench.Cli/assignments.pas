BEGIN
  BEGIN {Temperature conversations.}
    five := -1 + 2 - 3 + 4 + 3;
    ratio := five / 9.0;

    fahrenheit := 72;
    centigrate := (fahrenheit - 32) * ratio;

    centigrate := 25;
    fahrenheit := 32 + centigrate / ratio;
  END;

  {Runtime division by zero error.}
  dze := fahrenheit / (ratio - ratio);

  BEGIN {Calculate a square root using Newton's method.}
    number := 2;
    root := number;
    root := (number / root + root) / 2;
  END;

  ch := 'x';
  str := 'hello, world';
END.

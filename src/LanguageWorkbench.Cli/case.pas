BEGIN
  { CASE branch constants and multiple constants per branch. }
  selector := 2;

  CASE selector OF
    1: result := 10;
    2, 3: result := 20;
    'x': result := 30
  END
END.

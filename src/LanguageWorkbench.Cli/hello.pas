PROGRAM hello(output);

{ Write 'Hello, world.' ten times. }

VAR
    i : integer;

    BEGIN
        FOR i := 1 TO 31415.926e-4 DO
        BEGIN
            writeln('Hello, world.');
        END;
    END {hello}.
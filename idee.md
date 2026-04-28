interfaccia virtuale 2d statica

dialoghi consecutivi

personaggi ricorrenti

* snake
* manager
* ???

~~meccaniche di coding parallelo (sx dx) con targets~~

meccanica di coding in "hack" del codice

hack precisi o meno (abilità di aprirli dove si vuole? <5>)



comandi drag and drop con costo



comandi pseudo js?

* let numerico

&#x09;<2>

&#x09;{number n = 5}

* assegnamento

&#x09;<1>

&#x09;{n = 5}

* if(n (<|>|==|combin.) n)

&#x09;<2>

&#x09;{if n < m}

* else

&#x09;<1>

&#x09;{else}

* for i (j l m n...)

&#x09;<4>

&#x09;{for i from 0 to 4}

* let \[n] array di dim n

&#x09;<3>

&#x09;{number\[] array = \[5]}

* let \[n] array di dim n

&#x09;<3>

&#x09;{number\[] array = \[5]} (LOGICA HEAP!?!?)

* assegnamento in \[] o in generale

&#x09;<1>

&#x09;{array\[n] = 4}

* while(n (<|>|==|combin.) n)

&#x09;<3>

&#x09;{while n >= 0}

* break/skip

&#x09;<1>

&#x09;{break} {skip}

* return <1>
    {return} // l'idea è che lo usi solo in funzioni
* func <5> // ogni istruzione che contiene costa 1 in meno, e chiamarla è gratis
{
  func pow a b // 5 + 4 = 9
    number result = 1 // 1
    for i from 0 to b // 3
      result = result \* a // 0
    return result // 0
}



Operatori + - \* / % costano sempre 0

es. number power = energy \* 2



(target finali >= 0) + no loop + no error

INT 32 bit

NO scambi di tipi



sviluppi futuri

* char come int?
* bool da 0 e != 0?
* array complessi?




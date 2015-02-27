/**  Get duration total for property  **/

select 
sum(duration) 
from chargedcalls
where placedfrom = '(401) 433-3100'
and calltype not in ('On Net','Toll Free')

/***************************************/

/**  Gets the total duration and total charges  **/

SELECT SUM(duration), SUM(CAST(RIGHT(charge, LEN(charge) - 1) as float))
from chargedcalls
where placedfrom = '(401) 433-3100'
and calltype not in ('On Net','Toll Free')

/*************************************************/

/** use the following to browse thru the calls  **/

select * from chargedcalls
where placedfrom = '(401) 433-3100'
and calltype not in ('On Net','Toll Free')


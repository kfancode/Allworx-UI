With runningtotal as 
(
select 
placedfrom,
placedto,
calldate,
calltype,
calltime,
duration,
sum(duration) over (partition by placedfrom) as durTotal,
(sum(duration) over (partition by placedfrom)) * (cast(right(rate, len(rate)-1) as decimal(7,4))) as PropBill,
(cast(right(rate, len(rate)-1) as decimal(7,4))) as myRate
from chargedcalls
where calltype in ('Incoming Toll Free','domestic')

)

select * from runningtotal
--and placedto = @phonenumber or placedfrom = @phonenumber
order by calltype




/****


There are domestic calls that have no charge.  These calls are included in the minutes total.
This is because the call length is either 0.1 or 0.  Multiply that by the 0.0250 rate and that comes to 
0.0025.  With rounding this shows as no charge.  Thus minutes are included in total but there is no charge.


****/


/***   Run this to get the totals - use to check totals making sure they match invoice  ***/
select cast(sum(duration) as numeric(7,2)) 'Minutes',
case
when calltype = 'domestic' then round(sum(cast(duration as numeric(7,2)) * 0.0250),2)
when calltype = 'incoming toll free' then round(sum(cast(duration as numeric(7,2)) * 0.0400),2)
end  'Our Total'
from chargedcalls
where
calltype in ('domestic','Incoming Toll Free')
group by calltype


/***   run the following to file to create the tab delimited file to import into Excel   ***/

declare @phonenumber varchar(50) = '(215) 860-7985'

select 
calldate,
calltime,
calltype,
placedto,
placedfrom,
cast(duration as numeric(7,2)) 'Minutes',
right(rate, len(rate)-1) as rate,
case 
when calltype = 'domestic' then duration * 0.0250
when calltype = 'incoming toll free' then duration * 0.0400
end 'Our Charge',
acctcode
from chargedcalls
where calltype in ('Incoming Toll Free','domestic')
--and placedto = @phonenumber or placedfrom = @phonenumber
order by calltype


/*****   use the following to remove entries from DB - needs to be done before running totals for month  ******/
delete from chargedcalls where 1=1






 


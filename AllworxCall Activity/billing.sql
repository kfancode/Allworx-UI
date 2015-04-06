/****


There are domestic calls that have no charge.  These calls are included in the minutes total.
This is because the call length is either 0.1 or 0.  Multiply that by the 0.0250 rate and that comes to 
0.0025.  With rounding this shows as no charge.  Thus minutes are included in total but there is no charge.


****/


/***   Run this to get the domestic calls totals   ***/
select cast(sum(duration) as numeric(7,2)) 'Minutes', round(sum(cast(duration as numeric(7,2)) * 0.0250),2) 'Our Total'
from chargedcalls
where
calltype in ('domestic')
group by calltype


/***   run the following to file to create the tab delimited file to import into Excel   ***/
select 
calldate,
calltime,
calltype,
placedto,
placedfrom,
cast(duration as numeric(7,2)) 'Minutes',
rate,
case 
when calltype = 'domestic' then duration * 0.0250
when calltype = 'incoming toll free' then duration * 0.0400
end 'Our Charge',
acctcode
from chargedcalls
where calltype in ('Incoming Toll Free','domestic')
order by calltype






 


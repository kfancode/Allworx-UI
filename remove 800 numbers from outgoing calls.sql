/**  remove outgoing calls to 800 numbers - no need to bill  **/

-- find outgoing calls to 800 numbers

select * from outgoingcalls 
where placedto like '(800)%' 
or placedto like '(855)%' 
or placedto like '(866)%' 
or placedto like '(877)%' 
or placedto like  '(888)%' 

order by placedto

--  delete 800 numbers from outgoing list

delete from outgoingcalls 
where placedto like '(800)%' 
or placedto like '(855)%' 
or placedto like '(866)%' 
or placedto like '(877)%' 
or placedto like  '(888)%' 
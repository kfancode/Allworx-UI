/**  move Picerne's 800 number calls to their own table  **/


-- find all 800 calls in list
select * from incomingcalls where
placedto in ('(800) 323-1190', '(800) 742-3763')

-- copy 800 calls into pic800calls table
insert into dbo.pic800calls 
select duration, calltype, placedfrom, placedto 
	from incomingcalls where placedto in ('(800) 323-1190', '(800) 742-3763')

-- delete pic800calls from incoming calls table
delete from incomingcalls where placedto in ('(800) 323-1190', '(800) 742-3763')

--check to see if they are in the pic800calls table
	select * from pic800calls
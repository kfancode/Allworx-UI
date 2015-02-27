/** move forwarded calls to their own table  **/

-- copy forwarded calls into forwarded calls table
insert into dbo.forwardedcalls 
select duration, calltype, placedfrom, placedto 
	from incomingcalls where placedto like '%forwarded%'

-- delete forwarded calls from incoming calls table
delete from incomingcalls where placedto like '%forwarded%'

--check to see if they are in the forwarded calls table
	select * from forwardedcalls


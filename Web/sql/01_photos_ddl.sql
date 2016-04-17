-- Setup the Photos database

drop table photo;

drop table album;

create table album (
	album_id           	int not null primary key auto_increment,
	album_name			varchar(500) not null,
	location  			varchar(1000) not null
);

create table photo (
	photo_id 			int not null primary key auto_increment,
    album_id			int not null references album(album_id),
	filename 			varchar(1000) not null
);



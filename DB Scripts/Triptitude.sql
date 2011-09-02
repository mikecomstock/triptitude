USE [Triptitude_ActivityBranch]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AnonymousId] [varchar](100) NULL,
	[Email] [varchar](200) NULL,
	[HashedPassword] [varchar](200) NULL,
	[DefaultTrip_Id] [int] NULL,
	[Guid] [uniqueidentifier] NULL,
	[GuidCreatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Trips](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[User_Id] [int] NOT NULL,
	[Created_On] [datetime] NOT NULL,
	[BeginDate] [date] NULL,
	[Moderated] [bit] NOT NULL,
	[ShowInSiteMap] [bit] NOT NULL,
	[ShowInSite] [bit] NOT NULL,
 CONSTRAINT [PK_Trips] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Notes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Activity_Id] [int] NULL,
	[User_Id] [int] NOT NULL,
	[Created_On] [datetime] NOT NULL,
	[Text] [varchar](2000) NOT NULL,
	[Public] [bit] NOT NULL,
 CONSTRAINT [PK_ItineraryItemNote] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ItineraryItemId] ON [Notes] 
(
	[Activity_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Activities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Trip_Id] [int] NOT NULL,
	[BeginDay] [int] NOT NULL,
	[BeginTime] [time](7) NULL,
	[EndDay] [int] NOT NULL,
	[EndTime] [time](7) NULL,
 CONSTRAINT [PK_Activities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Items](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ASIN] [varchar](50) NULL,
	[DetailPageURL] [varchar](1000) NULL,
	[SmallImageURL] [varchar](1000) NULL,
	[SmallImageHeight] [decimal](3, 0) NULL,
	[SmallImageWidth] [decimal](3, 0) NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Countries](
	[GeoNameID] [int] NOT NULL,
	[ISO] [varchar](2) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[GeoNameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ActivityTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Activity_Id] [int] NOT NULL,
	[Tag_Id] [int] NOT NULL,
 CONSTRAINT [PK_ActivityTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Tags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UX_Tags] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Regions](
	[GeoNameID] [int] NOT NULL,
	[ASCIIName] [varchar](200) NOT NULL,
	[GeoNameAdmin1Code] [varchar](20) NULL,
	[Country_GeoNameId] [int] NOT NULL,
 CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED 
(
	[GeoNameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Country_GeoNameId] ON [Regions] 
(
	[Country_GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Cities](
	[GeoNameId] [int] NOT NULL,
	[ASCIIName] [varchar](200) NOT NULL,
	[Latitude] [decimal](9, 6) NOT NULL,
	[Longitude] [decimal](9, 6) NOT NULL,
	[Region_GeoNameId] [int] NOT NULL,
	[GeoPoint] [geography] NOT NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Region_GeoNameId] ON [Cities] 
(
	[Region_GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE SPATIAL INDEX [SX_GeoPoint] ON [Cities] 
(
	[GeoPoint]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Hotels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelsCombined_Id] [int] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Latitude] [decimal](9, 6) NOT NULL,
	[Longitude] [decimal](9, 6) NOT NULL,
	[GeoPoint] [geography] NOT NULL,
	[Image_Id] [int] NOT NULL,
	[NumberOfReviews] [int] NOT NULL,
	[ConsumerRating] [decimal](5, 2) NOT NULL,
	[NearestCity_GeoNameId] [int] NOT NULL,
 CONSTRAINT [PK_Hotels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE SPATIAL INDEX [IX_GeoPoint] ON [Hotels] 
(
	[GeoPoint]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_HotelsCombined_Id] ON [Hotels] 
(
	[HotelsCombined_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TransportationTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TransportationTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Places](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FactualId] [varchar](50) NULL,
	[Name] [varchar](300) NOT NULL,
	[Address] [varchar](300) NULL,
	[AddressExtended] [varchar](300) NULL,
	[POBox] [varchar](50) NULL,
	[Locality] [varchar](100) NULL,
	[Region] [varchar](100) NULL,
	[Country] [varchar](5) NULL,
	[PostCode] [varchar](50) NULL,
	[Telephone] [varchar](50) NULL,
	[Fax] [varchar](50) NULL,
	[Category] [varchar](200) NULL,
	[Website] [varchar](500) NULL,
	[Email] [varchar](300) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[GeoPoint] [geography] NULL,
	[Status] [varchar](10) NULL,
 CONSTRAINT [PK_Places] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_FactualId] ON [Places] 
(
	[FactualId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE SPATIAL INDEX [IX_GeoPoint] ON [Places] 
(
	[GeoPoint]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ItemTags](
	[Tag_Id] [int] NOT NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_ItemTags] PRIMARY KEY CLUSTERED 
(
	[Tag_Id] ASC,
	[Item_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PlaceActivities](
	[Id] [int] NOT NULL,
	[Place_Id] [int] NOT NULL,
 CONSTRAINT [PK_PlaceActivities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Place_Id] ON [PlaceActivities] 
(
	[Place_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TransportationActivities](
	[Id] [int] NOT NULL,
	[TransportationType_Id] [int] NOT NULL,
	[FromCity_GeoNameId] [int] NOT NULL,
	[ToCity_GeoNameId] [int] NOT NULL,
 CONSTRAINT [PK_Transportations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HotelActivities](
	[Id] [int] NOT NULL,
	[Hotel_Id] [int] NOT NULL,
 CONSTRAINT [PK_LodgingActivities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Hotel_Id] ON [HotelActivities] 
(
	[Hotel_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [Destinations] as
select GeoNameId Id, Name, GeoNameID Country_GeoNameId, null Region_GeoNameId, null City_GeoNameId from Countries
union
select  GeoNameId Id, ASCIIName Name, null Country_GeoNameId, GeoNameID Region_GeoNameId, null City_GeoNameId  from Regions
union
select GeoNameId Id, ASCIIName Name, null Country_GeoNameId,null Region_GeoNameId, GeoNameId City_GeoNameId  from cities
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [PlacesNear](@Latitude [decimal](9, 6), @Longitude [decimal](9, 6), @RadiusInMeters [int])
RETURNS TABLE AS 
return
	select id Place_Id,GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) Distance from Places
	where GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) <= @RadiusInMeters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [CitiesNear](@Latitude [decimal](9, 6), @Longitude [decimal](9, 6), @RadiusInMeters [int])
RETURNS TABLE AS 
return
	select geonameid City_GeoNameId,GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) Distance from Cities
	where GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) <= @RadiusInMeters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [InsertCity]
	@GeoNameId [int],
	@ASCIIName [nvarchar](200),
	@Latitude [decimal](9, 6),
	@Longitude [decimal](9, 6),
	@Region_GeoNameId [int]
WITH EXECUTE AS CALLER
AS
begin
	insert into Cities
	(GeoNameId, ASCIIName, Latitude, Longitude, GeoPoint, Region_GeoNameId)
	values (@GeoNameId, @ASCIIName, @Latitude, @Longitude, geography::Point(@Latitude, @Longitude, 4326), @Region_GeoNameId)
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [HotelsNear](@Latitude [decimal](9, 6), @Longitude [decimal](9, 6), @RadiusInMeters [int])
RETURNS TABLE AS 
return
	select id Hotel_Id,GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) Distance from Hotels
	where GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) <= @RadiusInMeters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [InsertHotel]
	@HotelsCombined_Id [int],
	@Name [varchar](200),
	@Latitude [decimal](9, 6),
	@Longitude [decimal](9, 6),
	@Image_Id [int],
	@NumberOfReviews [int],
	@ConsumerRating [decimal](5, 2)
WITH EXECUTE AS CALLER
AS
begin
	begin tran
		declare @ExistingId int
		select @ExistingId = Id from Hotels with(NOLOCK) where HotelsCombined_Id = @HotelsCombined_Id

		declare @NearestCityGeoNameId int
		set @NearestCityGeoNameId  = (select top 1 city_geonameid NearestCityGeoNameId from CitiesNear(@Latitude, @Longitude, 5000) order by distance)

		if @ExistingId is null
		begin
			insert into Hotels (HotelsCombined_Id, Name, Latitude, Longitude, GeoPoint, Image_Id, NumberOfReviews, ConsumerRating, NearestCity_GeoNameId)
			values (@HotelsCombined_Id, @Name,@Latitude,@Longitude,geography::Point(@Latitude, @Longitude, 4326), @Image_Id, @NumberOfReviews, @ConsumerRating, @NearestCityGeoNameId)
		end
		else
		begin
			update Hotels set
			Name = @Name,
			Latitude = @Latitude,
			Longitude = @Longitude,
			GeoPoint = geography::Point(@Latitude, @Longitude, 4326),
			Image_Id = @Image_Id,
			NumberOfReviews = @NumberOfReviews,
			ConsumerRating = @ConsumerRating
			where HotelsCombined_Id = @HotelsCombined_Id
		end
	commit
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [CityActivities] as
select ha.Id Activity_Id, h.NearestCity_GeoNameId City_GeoNameId from HotelActivities ha join Hotels h on ha.Hotel_Id = h.Id
union
select ta.Id Activity_Id, ta.City_GeoNameId City_GeoNameId from TagActivities ta
union
select ta.Id Activity_Id, ta.FromCity_GeoNameId City_GeoNameId from TransportationActivities ta
union
select ta.Id Activity_Id, ta.ToCity_GeoNameId City_GeoNameId from TransportationActivities ta
GO
ALTER TABLE [Trips] ADD  CONSTRAINT [DF_Trips_Moderated]  DEFAULT ((0)) FOR [Moderated]
GO
ALTER TABLE [Trips] ADD  CONSTRAINT [DF_Trips_ShowInSitemap]  DEFAULT ((0)) FOR [ShowInSiteMap]
GO
ALTER TABLE [Trips] ADD  CONSTRAINT [DF_Trips_ShowInSite]  DEFAULT ((0)) FOR [ShowInSite]
GO
ALTER TABLE [Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_DefaultTrips] FOREIGN KEY([DefaultTrip_Id])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [Users] CHECK CONSTRAINT [FK_Users_DefaultTrips]
GO
ALTER TABLE [Trips]  WITH CHECK ADD  CONSTRAINT [FK_Trips_Created_By] FOREIGN KEY([User_Id])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [Trips] CHECK CONSTRAINT [FK_Trips_Created_By]
GO
ALTER TABLE [Notes]  WITH CHECK ADD  CONSTRAINT [FK_Notes_Activities] FOREIGN KEY([Activity_Id])
REFERENCES [Activities] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Notes] CHECK CONSTRAINT [FK_Notes_Activities]
GO
ALTER TABLE [Activities]  WITH CHECK ADD  CONSTRAINT [FK_Activities_Trips] FOREIGN KEY([Trip_Id])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [Activities] CHECK CONSTRAINT [FK_Activities_Trips]
GO
ALTER TABLE [ActivityTags]  WITH CHECK ADD  CONSTRAINT [FK_ActivityTags_Activities] FOREIGN KEY([Activity_Id])
REFERENCES [Activities] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [ActivityTags] CHECK CONSTRAINT [FK_ActivityTags_Activities]
GO
ALTER TABLE [ActivityTags]  WITH CHECK ADD  CONSTRAINT [FK_ActivityTags_Tags] FOREIGN KEY([Tag_Id])
REFERENCES [Tags] ([Id])
GO
ALTER TABLE [ActivityTags] CHECK CONSTRAINT [FK_ActivityTags_Tags]
GO
ALTER TABLE [ItemTags]  WITH CHECK ADD  CONSTRAINT [FK_ItemTags_Items] FOREIGN KEY([Item_Id])
REFERENCES [Items] ([Id])
GO
ALTER TABLE [ItemTags] CHECK CONSTRAINT [FK_ItemTags_Items]
GO
ALTER TABLE [ItemTags]  WITH CHECK ADD  CONSTRAINT [FK_ItemTags_Tags] FOREIGN KEY([Tag_Id])
REFERENCES [Tags] ([Id])
GO
ALTER TABLE [ItemTags] CHECK CONSTRAINT [FK_ItemTags_Tags]
GO
ALTER TABLE [PlaceActivities]  WITH CHECK ADD  CONSTRAINT [FK_PlaceActivities_Places] FOREIGN KEY([Place_Id])
REFERENCES [Places] ([Id])
GO
ALTER TABLE [PlaceActivities] CHECK CONSTRAINT [FK_PlaceActivities_Places]
GO
ALTER TABLE [TransportationActivities]  WITH CHECK ADD  CONSTRAINT [FK_Transportations_TransportationTypes] FOREIGN KEY([TransportationType_Id])
REFERENCES [TransportationTypes] ([Id])
GO
ALTER TABLE [TransportationActivities] CHECK CONSTRAINT [FK_Transportations_TransportationTypes]
GO
ALTER TABLE [HotelActivities]  WITH CHECK ADD  CONSTRAINT [FK_LodgingActivities_Hotels] FOREIGN KEY([Hotel_Id])
REFERENCES [Hotels] ([Id])
GO
ALTER TABLE [HotelActivities] CHECK CONSTRAINT [FK_LodgingActivities_Hotels]
GO

USE [master]
GO
CREATE DATABASE [PRN222_bookshop]
 
GO
ALTER DATABASE [PRN222_bookshop] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET ARITHABORT OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PRN222_bookshop] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PRN222_bookshop] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PRN222_bookshop] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PRN222_bookshop] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET RECOVERY FULL 
GO
ALTER DATABASE [PRN222_bookshop] SET  MULTI_USER 
GO
ALTER DATABASE [PRN222_bookshop] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PRN222_bookshop] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PRN222_bookshop] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PRN222_bookshop] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PRN222_bookshop] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PRN222_bookshop] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'PRN222_bookshop', N'ON'
GO
ALTER DATABASE [PRN222_bookshop] SET QUERY_STORE = ON
GO
ALTER DATABASE [PRN222_bookshop] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PRN222_bookshop]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Books](
	[BookID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](255) NOT NULL,
	[Author] [varchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [nvarchar](max) NULL,
	[Stock] [int] NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[DownloadLink] [varchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdateAt] [datetime] NULL,
	[Type] [int] NULL,
	[CategoryID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BookID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chat](
	[ChatID] [int] IDENTITY(1,1) NOT NULL,
	[ReceiverID] [int] NOT NULL,
	[SenderID] [int] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ChatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[BookID] [int] NOT NULL,
	[Quantity] [int] NULL,
	[Subtotal] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[OrderStatus] [int] NULL,
	[OrderDate] [datetime] NULL,
	[Address] [nvarchar](max) NULL,
	[Phone] nvarchar(20) NULL,
	[Preferences] [nvarchar](max) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[BookID] [int] NOT NULL,
	[Ratting] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[CreateAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](400) NOT NULL,
	[Role] [int] NOT NULL,
	[Preferences] [nvarchar](max) NULL,
	[CreateAt] [datetime] NULL,
	[Address] [nvarchar](max) NULL,
	[Status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wishlist](
	[UserID] [int] NOT NULL,
	[BookID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[BookID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Books] ON 

INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (1, N'To Kill a Mockingbird', N'Harper Lewww', N'A classic novel about racial injustice', N'/uploads/56cd2f4e-3874-4fb9-8f01-ebe53d97e4d3.jpg', 10, CAST(15.99 AS Decimal(18, 2)), N'https://example.com/to-kill-a-mockingbird.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 1, 1)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (2, N'1984', N'George Orwell', N'A dystopian novel about totalitarianism', N'/uploads/eb24ee89-0a55-42ae-a9fc-3f6b764e22e9.jpg', 15, CAST(12.99 AS Decimal(18, 2)), N'https://example.com/1984.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 2, 2)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (3, N'The Great Gatsby', N'F. Scott Fitzgerald', N'A novel about the American Dream', N'/uploads/4d18c65c-b99f-48a3-a115-5e9be7efd2d8.jpg', 20, CAST(14.99 AS Decimal(18, 2)), N'https://example.com/the-great-gatsby.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 3, 3)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (4, N'The Girl with the Dragon Tattoo', N'Stieg Larsson', N'A mystery novel about corruption', N'/uploads/c636b0bc-c007-4afc-b836-d47edf00df9d.jpg', 25, CAST(16.99 AS Decimal(18, 2)), N'https://example.com/the-girl-with-the-dragon-tattoo.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 4, 4)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (5, N'The Hunger Games', N'Suzanne Collins', N'A dystopian novel about rebellion', N'/uploads/42c0df92-81c1-4a22-b019-f7b5d2e8b9ec.jpg', 30, CAST(18.99 AS Decimal(18, 2)), N'https://example.com/the-hunger-games.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 5, 5)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (6, N'Pride and Prejudice', N'Jane Austen', N'A romance novel about love', N'/uploads/0818feb2-f3a9-4e56-aadf-0b2b803d4b6d.jpg', 35, CAST(10.99 AS Decimal(18, 2)), N'https://example.com/pride-and-prejudice.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 6, 6)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (7, N'The Lord of the Rings', N'J.R.R. Tolkien', N'A fantasy novel about friendship', N'/uploads/2389ddb5-4a6f-46b4-af2e-8806716094bd.jpg', 40, CAST(19.99 AS Decimal(18, 2)), N'https://example.com/the-lord-of-the-rings.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 7, 7)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (8, N'The Shining', N'Stephen King', N'A horror novel about isolation', N'/uploads/1aced5f2-b2a0-4949-9bde-df5f80ce382e.jpg', 45, CAST(12.99 AS Decimal(18, 2)), N'https://example.com/the-shining.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 8, 8)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (9, N'The Alchemist', N'Paulo Coelho', N'A self-help novel about spirituality', N'/uploads/ff4f9984-c11d-49c2-9102-6e2e588b714d.jpg', 50, CAST(14.99 AS Decimal(18, 2)), N'https://example.com/the-alchemist.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 9, 9)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (10, N'The 7 Habits of Highly Effective People', N'Stephen Covey', N'A self-help book about personal development', N'/uploads/1bc32345-a25f-430c-809c-4340f0409848.jpg', 55, CAST(16.99 AS Decimal(18, 2)), N'https://example.com/the-7-habits-of-highly-effective-people.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 10, 10)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (11, N'The Joy of Cooking', N'Irma S. Rombauer', N'A cookbook about cooking', N'/uploads/b5a39391-0273-4b4a-b694-084ea35c2a34.jpg', 60, CAST(18.99 AS Decimal(18, 2)), N'https://example.com/the-joy-of-cooking.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 11, 11)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (12, N'The Art of War', N'Sun Tzu', N'A book about strategy and tactics', N'/uploads/ca45b3a5-a41a-4e62-90f5-1bc8c7a81a5e.jpg', 65, CAST(10.99 AS Decimal(18, 2)), N'https://example.com/the-art-of-war.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 12, 12)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (13, N'The Picture of Dorian Gray', N'Oscar Wilde', N'A novel about vanity and morality', N'/uploads/4395f89b-8f16-4304-ba75-5341811c693b.jpg', 70, CAST(12.99 AS Decimal(18, 2)), N'https://example.com/the-picture-of-dorian-gray.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 13, 13)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (14, N'The Stranger', N'Albert Camus', N'A novel about alienation and morality', N'/uploads/cd53da86-072c-4344-b440-f7f816c4a666.jpg', 75, CAST(14.99 AS Decimal(18, 2)), N'https://example.com/the-stranger.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 14, 14)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (15, N'The Catcher in the Rye', N'J.D. Salinger', N'A novel about teenage angst', N'/uploads/988b7fa2-06eb-4054-91c3-9dc6e385234f.jpg', 80, CAST(16.99 AS Decimal(18, 2)), N'https://example.com/the-catcher-in-the-rye.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 15, 15)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (16, N'The Bell Jar', N'Sylvia Plath', N'A novel about mental illness', N'/uploads/7746b792-4d6c-4fc7-a627-011ba804e76d.jpg', 85, CAST(18.99 AS Decimal(18, 2)), N'https://example.com/the-bell-jar.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 16, 16)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (17, N'The Color Purple', N'Alice Walker', N'A novel about racism and sexism', N'/uploads/e9f3a928-32e7-4e34-ac34-2adc1de0bb24.jpg', 90, CAST(19.99 AS Decimal(18, 2)), N'https://example.com/the-color-purple.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 17, 17)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (18, N'The Handmaid''s Tale', N'Margaret Atwood', N'A dystopian novel about oppression', N'/uploads/29417c46-5c88-478e-88a0-47085c212cde.jpg', 95, CAST(20.99 AS Decimal(18, 2)), N'https://example.com/the-handmaid''s-tale.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 18, 18)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (19, N'The Nightingale', N'Kristin Hannah', N'A historical novel about love and loss', N'/uploads/c356aec0-bc7e-4a0d-b4aa-8200d9080214.jpg', 100, CAST(22.99 AS Decimal(18, 2)), N'https://example.com/the-nightingale.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 19, 19)
INSERT [dbo].[Books] ([BookID], [Title], [Author], [Description], [Image], [Stock], [Price], [DownloadLink], [CreatedAt], [UpdateAt], [Type], [CategoryID]) VALUES (20, N'The Hate U Give', N'Angie Thomas', N'A young adult novel about racism and activism', N'/uploads/4ed7b0cd-658e-430d-9455-acfcc2733780.jpg', 105, CAST(24.99 AS Decimal(18, 2)), N'https://example.com/the-hate-u-give.pdf', CAST(N'2025-02-05T13:54:36.743' AS DateTime), CAST(N'2025-02-05T13:54:36.743' AS DateTime), 20, 20)
SET IDENTITY_INSERT [dbo].[Books] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (1, N'Fiction')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (2, N'Non-Fiction')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (3, N'Biography')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (4, N'Mystery')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (5, N'Thriller')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (6, N'Romance')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (7, N'Science Fiction')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (8, N'Fantasy')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (9, N'Horror')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (10, N'Historical Fiction')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (11, N'Self-Help')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (12, N'Cookbook')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (13, N'Travel')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (14, N'Business')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (15, N'Technology')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (16, N'Art')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (17, N'Photography')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (18, N'Music')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (19, N'Poetry')
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (20, N'Children''s Book')
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Chat] ON 

INSERT [dbo].[Chat] ([ChatID], [ReceiverID], [SenderID], [Message], [Timestamp]) VALUES (1, 2, 3, N'xin chao', CAST(N'2025-02-05T14:04:44.130' AS DateTime))
INSERT [dbo].[Chat] ([ChatID], [ReceiverID], [SenderID], [Message], [Timestamp]) VALUES (2, 3, 2, N'xin chào', CAST(N'2025-02-05T14:05:04.063' AS DateTime))
SET IDENTITY_INSERT [dbo].[Chat] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (1, N'User')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (2, N'Staff')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (3, N'Admin')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (1, N'admin', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'admin@example.com', 3, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'567 Walnut St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (2, N'huybach', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'huybach@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'123 Main St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (3, N'khamhung', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'khamhung@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'456 Elm St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (4, N'staff1', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'bob.smith@example.com', 2, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'789 Oak St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (5, N'staff2', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'alice.johnson@example.com', 2, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'321 Maple St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (6, N'staff3', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'mike.brown@example.com', 2, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'901 Pine St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (7, N'staff4', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'emily.davis@example.com', 2, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'234 Cedar St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (8, N'tienthanh', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'tienthanh@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'890 Cherry St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (9, N'tansang', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'tansang@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'345 Spruce St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (10, N'huyviet', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'huyviet@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'678 Fir St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (11, N'WilliamHarris', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'william.harris@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'123 Cypress St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (12, N'IsabellaWalker', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'isabella.walker@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'456 Hickory St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (13, N'JamesHall', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'james.hall@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'789 Sycamore St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (14, N'SophiaKim', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'sophia.kim@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'901 Magnolia St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (15, N'BenjaminPatel', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'benjamin.patel@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'234 Rowan St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (16, N'MiaGarcia', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'mia.garcia@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'567 Ash St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (17, N'AlexanderRamos', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'alexander.ramos@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'890 Ash St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (18, N'CharlotteWong', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'charlotte.wong@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'345 Ash St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (19, N'EthanKim', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'ethan.kim@example.com', 1, N'Default', CAST(N'2025-02-05T13:54:36.857' AS DateTime), N'678 Ash St', 1)
INSERT [dbo].[Users] ([UserID], [Username], [Password], [Email], [Role], [Preferences], [CreateAt], [Address], [Status]) VALUES (1002, N'cuong', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', N'cuong@example.com', 1, NULL, CAST(N'2025-02-14T07:36:41.233' AS DateTime), NULL, 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Books] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Books] ADD  DEFAULT (getdate()) FOR [UpdateAt]
GO
ALTER TABLE [dbo].[Chat] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[Books]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Chat]  WITH CHECK ADD FOREIGN KEY([ReceiverID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Chat]  WITH CHECK ADD FOREIGN KEY([SenderID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([BookID])
REFERENCES [dbo].[Books] ([BookID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([BookID])
REFERENCES [dbo].[Books] ([BookID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([Role])
REFERENCES [dbo].[Role] ([RoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wishlist]  WITH CHECK ADD FOREIGN KEY([BookID])
REFERENCES [dbo].[Books] ([BookID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wishlist]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
USE [master]
GO
ALTER DATABASE [PRN222_bookshop] SET  READ_WRITE 
GO

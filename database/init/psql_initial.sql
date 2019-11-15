CREATE TABLE "User"
(
  "Id" SERIAL,
  "Email" varchar(255),
  "TempoToken" varchar(255),
  "TogglToken" varchar(255),
  CONSTRAINT "PK_User" PRIMARY KEY ("Id")
);
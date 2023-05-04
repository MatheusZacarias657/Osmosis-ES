create table Doctors
(
    Name              varchar(500)  not null,
    specialty         varchar(500)  not null,
    entryTime         time          not null,
    departureTime     time          not null,
    active            bit default 1 not null,
    id                int identity
        primary key,
    appointmentPeriod time          not null
)
go

create table DailyAppointments
(
    startTime time not null,
    id        int identity
        primary key,
    id_doctor int  not null
        constraint id_doctor_dailyAppointments___fk
            references Doctors
)
go

CREATE TRIGGER [updateDailyApointments]
ON [Doctors]
WITH EXECUTE AS CALLER
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @status AS bit
    SELECT @status = active FROM INSERTED
    DECLARE @id_doctor AS INT
    SELECT @id_doctor = id FROM INSERTED

    IF @status = 0
    BEGIN
        DELETE FROM DailyAppointments WHERE id_doctor = @id_doctor
    END
    ELSE
    BEGIN
        DECLARE @period AS TIME
        DECLARE @appointmentPeriod AS TIME
        SELECT @appointmentPeriod = appointmentPeriod FROM INSERTED
        DECLARE @departureTime AS TIME
        SELECT @departureTime = departureTime FROM INSERTED

        DECLARE @appointmentsCount AS INT
        SELECT @appointmentsCount = COUNT(DailyAppointments.id)
        FROM DailyAppointments
            INNER JOIN INSERTED ON INSERTED.id = DailyAppointments.id_doctor

        IF @appointmentsCount = 0
        BEGIN
            SELECT @period = entryTime FROM INSERTED
            WHILE @departureTime > dbo.calcRegisterDailyApointMents (@appointmentPeriod, @period)
            BEGIN
                INSERT INTO DailyAppointments (startTime, id_doctor) VALUES (@period, @id_doctor)
                SET @period = dbo.calcRegisterDailyApointMents (@appointmentPeriod, @period)
            END
        END
        ELSE
        BEGIN
            DELETE FROM DailyAppointments WHERE id_doctor = @id_doctor
            SELECT @period = entryTime FROM INSERTED
            WHILE @departureTime > dbo.calcRegisterDailyApointMents (@appointmentPeriod, @period)
            BEGIN
                INSERT INTO DailyAppointments (startTime, id_doctor) VALUES (@period, @id_doctor)
                SET @period = dbo.calcRegisterDailyApointMents (@appointmentPeriod, @period)
            END
        END
    END
END
go

create table LogTypes
(
    name varchar(500) not null,
    id   int identity
        primary key
)
go

create table Logs
(
    id         int identity
        primary key,
    id_type    int          not null
        constraint typeLogs___fk
            references LogTypes,
    message    varchar(500) not null,
    exception  varchar(5000),
    stackTrace varchar(500) not null,
    [user]     varchar(500)
)
go

create table Status
(
    name           varchar(500)  not null,
    id             int identity
        primary key,
    createDocument bit default 0 not null
)
go

create table Appointments
(
    patientName     varchar(500) not null,
    id              int identity
        primary key,
    appointmentTime datetime     not null,
    id_doctor       int          not null
        constraint id_doctor_appointments___fk
            references Doctors,
    patientDocument varchar(500) not null,
    id_status       int          not null
        constraint id_status_Appointments___fk
            references Status
)
go

create table UserRoles
(
    name varchar(500) not null,
    id   int identity
        primary key
)
go

create table Users
(
    name     varchar(500)  not null,
    id       int identity
        primary key,
    login    varchar(500)  not null,
    password varchar(1000) not null,
    id_role  int           not null
        constraint id_role_Users___fk
            references UserRoles,
    active   bit default 1 not null,
    email    varchar(100)  not null
)
go

create table ActiveGuids
(
    guid           varchar(500) not null,
    id             int identity
        primary key,
    id_user        int
        constraint id_user_ActiveGuids___fk
            references Users,
    creationDate   datetime     not null,
    expirationDate datetime
)
go

CREATE TRIGGER [updateExpirationDate]
ON [ActiveGuids]
WITH EXECUTE AS CALLER
AFTER INSERT
AS
BEGIN
  DECLARE @validade AS int
  SET @validade = 7

  UPDATE ActiveGuids
  SET ActiveGuids.expirationDate = DATEADD(DAY, @validade, INSERTED.[creationDate])
  FROM INSERTED
END
go

CREATE FUNCTION dbo.calcRegisterDailyApointMents(@startTime TIME, @appointmentPeriod TIME)
RETURNS TIME
AS
BEGIN
    DECLARE @period AS DATETIME
    SET @period = DATEADD(HOUR, DATEPART(hh,@appointmentPeriod), @startTime)
    SET @period = DATEADD(MINUTE, DATEPART(mi,@appointmentPeriod), @period)
    SET @period = DATEADD(SECOND, DATEPART(ss,@appointmentPeriod), @period)

    RETURN CAST(@period AS time)
END
go

INSERT INTO Projeto_Leles.dbo.Status (name, id, createDocument) VALUES (N'ativa', 1, 0);
INSERT INTO Projeto_Leles.dbo.Status (name, id, createDocument) VALUES (N'concluída', 2, 0);
INSERT INTO Projeto_Leles.dbo.Status (name, id, createDocument) VALUES (N'remarcada', 3, 1);
INSERT INTO Projeto_Leles.dbo.Status (name, id, createDocument) VALUES (N'cancelada', 4, 0);
INSERT INTO Projeto_Leles.dbo.Status (name, id, createDocument) VALUES (N'não compareceu', 5, 0);

INSERT INTO Projeto_Leles.dbo.UserRoles (name, id) VALUES (N'admin', 1);
INSERT INTO Projeto_Leles.dbo.UserRoles (name, id) VALUES (N'user', 2);

INSERT INTO Projeto_Leles.dbo.LogTypes (name, id) VALUES (N'Action', 1);
INSERT INTO Projeto_Leles.dbo.LogTypes (name, id) VALUES (N'Warning', 2);
INSERT INTO Projeto_Leles.dbo.LogTypes (name, id) VALUES (N'Error', 3);


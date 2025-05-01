USE [pmsdb];

INSERT INTO [Company] VALUES ('CCL', 'Courtaulds Clothing Lanka');

INSERT INTO [CompanySequence] VALUES ('CCL', 1, 1, 0);

INSERT INTO [UserGroup] VALUES ('CCL-1', 'Administrator', 'CCL'), ('CCL-2', 'Worker', 'CCL');

INSERT INTO [UserShadow] VALUES ('CCL-0001', 'isiraratnayake1999@gmail.com', '$2a$11$rW8xJS9OTICyB.RVsSQk2Opi45SfKXPvp88AXBGwZuqnl4tMF9coa', 'Isira Uvindu Ratnayake', 'CCL-1', 'CCL-1', 'CCL-1', 'INSERT', '2025-04-27 10:20:00', 'CCL-0001');

INSERT INTO [User] VALUES ('CCL-0001', 'isiraratnayake1999@gmail.com', '$2a$11$rW8xJS9OTICyB.RVsSQk2Opi45SfKXPvp88AXBGwZuqnl4tMF9coa', 'Isira Uvindu Ratnayake', 'CCL-1', 'CCL-1', 'CCL-1');

INSERT INTO [TaskStatus] VALUES ('CCL-1', 'To Do', 'CCL'), ('CCL-2', 'In Progress', 'CCL'), ('CCL-3', 'In Review', 'CCL'), ('CCL-4', 'Done', 'CCL');
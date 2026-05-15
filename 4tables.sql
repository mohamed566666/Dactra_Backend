USE [Rujta];
GO
SET NOCOUNT ON;
GO


-- ============================================================
-- 1) Companies
-- ============================================================
INSERT INTO Companies (Name, CreatedAt, UpdatedAt) VALUES
(N'Pfizer',                GETDATE(), GETDATE()),
(N'Novartis',              GETDATE(), GETDATE()),
(N'Roche',                 GETDATE(), GETDATE()),
(N'Sanofi',                GETDATE(), GETDATE()),
(N'GlaxoSmithKline',       GETDATE(), GETDATE()),
(N'Johnson & Johnson',     GETDATE(), GETDATE()),
(N'Merck & Co.',           GETDATE(), GETDATE()),
(N'AstraZeneca',           GETDATE(), GETDATE()),
(N'Bayer',                 GETDATE(), GETDATE()),
(N'Abbott Laboratories',   GETDATE(), GETDATE()),
(N'Eli Lilly',             GETDATE(), GETDATE()),
(N'Bristol-Myers Squibb',  GETDATE(), GETDATE()),
(N'Boehringer Ingelheim',  GETDATE(), GETDATE()),
(N'Takeda',                GETDATE(), GETDATE()),
(N'Teva Pharmaceuticals',  GETDATE(), GETDATE()),
(N'EIPICO',                GETDATE(), GETDATE()),
(N'Pharco',                GETDATE(), GETDATE()),
(N'Amoun',                 GETDATE(), GETDATE()),
(N'Hikma Pharmaceuticals', GETDATE(), GETDATE()),
(N'Sigma Pharmaceutical',  GETDATE(), GETDATE()),
(N'Global Napi Pharma',    GETDATE(), GETDATE()),
(N'Mash Premiere',         GETDATE(), GETDATE()),
(N'Multipharma',           GETDATE(), GETDATE()),
(N'Memphis Pharmaceutical',GETDATE(), GETDATE()),
(N'Cipla',                 GETDATE(), GETDATE()),
(N'Sun Pharma',            GETDATE(), GETDATE()),
(N'Dr. Reddy''s',          GETDATE(), GETDATE()),
(N'Mylan',                 GETDATE(), GETDATE()),
(N'Servier',               GETDATE(), GETDATE()),
(N'Lundbeck',              GETDATE(), GETDATE());
GO

-- ============================================================
-- 2) Categories
-- ============================================================
INSERT INTO Categories (Name, CreatedAt, UpdatedAt) VALUES
(N'Analgesics',             GETDATE(), GETDATE()),
(N'NSAIDs',                 GETDATE(), GETDATE()),
(N'Antibiotics',            GETDATE(), GETDATE()),
(N'Antivirals',             GETDATE(), GETDATE()),
(N'Antifungals',            GETDATE(), GETDATE()),
(N'Antihistamines',         GETDATE(), GETDATE()),
(N'Antidiabetics',          GETDATE(), GETDATE()),
(N'Antihypertensives',      GETDATE(), GETDATE()),
(N'Antidepressants',        GETDATE(), GETDATE()),
(N'Antianxiety',            GETDATE(), GETDATE()),
(N'Cardiovascular',         GETDATE(), GETDATE()),
(N'Gastrointestinal',       GETDATE(), GETDATE()),
(N'Respiratory',            GETDATE(), GETDATE()),
(N'Dermatological',         GETDATE(), GETDATE()),
(N'Hormones',               GETDATE(), GETDATE()),
(N'Vitamins & Supplements', GETDATE(), GETDATE()),
(N'Anticoagulants',         GETDATE(), GETDATE()),
(N'Diuretics',              GETDATE(), GETDATE()),
(N'Corticosteroids',        GETDATE(), GETDATE()),
(N'Opioid Analgesics',      GETDATE(), GETDATE()),
(N'Antiparasitics',         GETDATE(), GETDATE()),
(N'Ophthalmic',             GETDATE(), GETDATE()),
(N'Statins',                GETDATE(), GETDATE()),
(N'PPI',                    GETDATE(), GETDATE()),
(N'Antiplatelets',          GETDATE(), GETDATE());
GO

-- ============================================================
-- 4) Medicines
-- ============================================================
DECLARE @Bases TABLE (
    Idx INT IDENTITY(1,1),
    BaseName         NVARCHAR(200),
    ActiveIngredient NVARCHAR(200),
    Smiles           NVARCHAR(500),
    CategoryName     NVARCHAR(100),
    Description      NVARCHAR(1000),
    ImageUrl         NVARCHAR(500)
);

INSERT INTO @Bases (BaseName, ActiveIngredient, Smiles, CategoryName, Description, ImageUrl) VALUES
(N'Panadol',      N'Paracetamol',               N'CC(=O)NC1=CC=C(O)C=C1',                                       N'Analgesics',        N'Pain reliever and fever reducer used for headaches, muscle aches and fever.',                          N'https://upload.wikimedia.org/wikipedia/commons/thumb/8/85/Paracetamol-skeletal.svg/1200px-Paracetamol-skeletal.svg.png'),
(N'Brufen',       N'Ibuprofen',                 N'CC(C)CC1=CC=C(C=C1)C(C)C(=O)O',                               N'NSAIDs',            N'Non-steroidal anti-inflammatory drug for pain, inflammation and fever.',                              N'https://upload.wikimedia.org/wikipedia/commons/thumb/6/6f/Ibuprofen.svg/1200px-Ibuprofen.svg.png'),
(N'Aspirin',      N'Acetylsalicylic Acid',      N'CC(=O)OC1=CC=CC=C1C(=O)O',                                    N'Antiplatelets',     N'Analgesic, antipyretic, anti-inflammatory and antiplatelet agent.',                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Aspirin-skeletal.svg/1200px-Aspirin-skeletal.svg.png'),
(N'Cataflam',     N'Diclofenac Potassium',      N'OC(=O)Cc1ccccc1Nc1c(Cl)cccc1Cl',                              N'NSAIDs',            N'NSAID used to relieve pain, swelling and joint stiffness.',                                           N'https://upload.wikimedia.org/wikipedia/commons/thumb/8/86/Diclofenac.svg/1200px-Diclofenac.svg.png'),
(N'Voltaren',     N'Diclofenac Sodium',         N'OC(=O)Cc1ccccc1Nc1c(Cl)cccc1Cl',                              N'NSAIDs',            N'NSAID for arthritis and acute musculoskeletal pain.',                                                 N'https://upload.wikimedia.org/wikipedia/commons/thumb/8/86/Diclofenac.svg/1200px-Diclofenac.svg.png'),
(N'Naproxen',     N'Naproxen',                  N'COc1ccc2cc(C(C)C(=O)O)ccc2c1',                                N'NSAIDs',            N'NSAID used for pain, menstrual cramps and inflammatory diseases.',                                    N'https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/Naproxen.svg/1200px-Naproxen.svg.png'),
(N'Celebrex',     N'Celecoxib',                 N'CC1=CC=C(C=C1)C2=CC(=NN2C3=CC=C(C=C3)S(=O)(=O)N)C(F)(F)F',    N'NSAIDs',            N'COX-2 selective NSAID for arthritis and acute pain.',                                                 N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/9c/Celecoxib.svg/1200px-Celecoxib.svg.png'),
(N'Augmentin',    N'Amoxicillin + Clavulanate', N'CC1(C)SC2C(NC(=O)C(N)c3ccc(O)cc3)C(=O)N2C1C(=O)O',            N'Antibiotics',       N'Broad-spectrum penicillin antibiotic for bacterial infections.',                                       N'https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Amoxicillin.svg/1200px-Amoxicillin.svg.png'),
(N'Amoxil',       N'Amoxicillin',               N'CC1(C)SC2C(NC(=O)C(N)c3ccc(O)cc3)C(=O)N2C1C(=O)O',            N'Antibiotics',       N'Penicillin antibiotic for ear, nose, throat and urinary tract infections.',                            N'https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Amoxicillin.svg/1200px-Amoxicillin.svg.png'),
(N'Zithromax',    N'Azithromycin',              N'CCC1OC(=O)C(C)C(O)C(C)C(OC2OC(C)CC(N(C)C)C2O)C(C)C(OC2CC(C)(OC)C(O)C(C)O2)C(C)(O)C1', N'Antibiotics', N'Macrolide antibiotic for respiratory, skin and ear infections.',                                       N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/04/Azithromycin_structure.svg/1200px-Azithromycin_structure.svg.png'),
(N'Klacid',       N'Clarithromycin',            N'CCC1OC(=O)C(C)C(OC2CC(C)(OC)C(O)C(C)O2)C(C)C(OC2OC(C)CC(N(C)C)C2O)C(C)(OC)CC(C)C(=O)C1', N'Antibiotics', N'Macrolide antibiotic for respiratory and H. pylori eradication.',                                      N'https://upload.wikimedia.org/wikipedia/commons/thumb/2/2b/Clarithromycin.svg/1200px-Clarithromycin.svg.png'),
(N'Ciprobay',     N'Ciprofloxacin',             N'O=C(O)c1cn(C2CC2)c2cc(N3CCNCC3)c(F)cc2c1=O',                  N'Antibiotics',       N'Fluoroquinolone antibiotic for urinary tract and respiratory infections.',                            N'https://upload.wikimedia.org/wikipedia/commons/thumb/3/30/Ciprofloxacin.svg/1200px-Ciprofloxacin.svg.png'),
(N'Tavanic',      N'Levofloxacin',              N'CC1COc2c(N3CCN(C)CC3)c(F)cc3c(=O)c(C(=O)O)cn1c23',            N'Antibiotics',       N'Broad-spectrum fluoroquinolone antibiotic.',                                                          N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/01/Levofloxacin.svg/1200px-Levofloxacin.svg.png'),
(N'Flagyl',       N'Metronidazole',             N'Cc1ncc([N+](=O)[O-])n1CCO',                                   N'Antibiotics',       N'Antibiotic and antiprotozoal for anaerobic infections.',                                              N'https://upload.wikimedia.org/wikipedia/commons/thumb/d/d1/Metronidazole.svg/1200px-Metronidazole.svg.png'),
(N'Bactrim',      N'Sulfamethoxazole + TMP',    N'Cc1cc(NS(=O)(=O)c2ccc(N)cc2)no1',                             N'Antibiotics',       N'Combination antibiotic for urinary tract infections and pneumonia.',                                  N'https://upload.wikimedia.org/wikipedia/commons/thumb/1/1c/Sulfamethoxazole.svg/1200px-Sulfamethoxazole.svg.png'),
(N'Tamiflu',      N'Oseltamivir',               N'CCC(CC)OC1C=C(C(=O)OCC)CC(NC(C)=O)C1N',                       N'Antivirals',        N'Antiviral medication used to treat and prevent influenza A and B.',                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Oseltamivir.svg/1200px-Oseltamivir.svg.png'),
(N'Zovirax',      N'Acyclovir',                 N'OCCOCn1cnc2c1ncn2N',                                          N'Antivirals',        N'Antiviral used for herpes simplex, varicella and zoster infections.',                                 N'https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Aciclovir.svg/1200px-Aciclovir.svg.png'),
(N'Diflucan',     N'Fluconazole',               N'OC(Cn1cncn1)(Cn1cncn1)c1ccc(F)cc1F',                          N'Antifungals',       N'Antifungal medication used for candidiasis and other fungal infections.',                             N'https://upload.wikimedia.org/wikipedia/commons/thumb/7/76/Fluconazole.svg/1200px-Fluconazole.svg.png'),
(N'Nizoral',      N'Ketoconazole',              N'CC(=O)N1CCN(c2ccc(OCC3COC(Cn4ccnc4)(c4ccc(Cl)cc4Cl)O3)cc2)CC1', N'Antifungals',     N'Antifungal medication for skin, scalp and systemic fungal infections.',                               N'https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Ketoconazole.svg/1200px-Ketoconazole.svg.png'),
(N'Claritine',    N'Loratadine',                N'CCOC(=O)N1CCC(=C2c3ccc(Cl)cc3CCc3cccnc32)CC1',                N'Antihistamines',    N'Second-generation antihistamine for allergies and hay fever.',                                        N'https://upload.wikimedia.org/wikipedia/commons/thumb/6/68/Loratadine.svg/1200px-Loratadine.svg.png'),
(N'Zyrtec',       N'Cetirizine',                N'OC(=O)COCCN1CCN(C(c2ccccc2)c2ccc(Cl)cc2)CC1',                 N'Antihistamines',    N'Antihistamine for allergic rhinitis and chronic urticaria.',                                          N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/03/Cetirizine.svg/1200px-Cetirizine.svg.png'),
(N'Allegra',      N'Fexofenadine',              N'CC(C)(C(=O)O)c1ccc(C(O)CCCN2CCC(C(O)(c3ccccc3)c3ccccc3)CC2)cc1', N'Antihistamines', N'Non-sedating antihistamine for allergic rhinitis and urticaria.',                                     N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/97/Fexofenadine.svg/1200px-Fexofenadine.svg.png'),
(N'Glucophage',   N'Metformin HCl',             N'CN(C)C(=N)NC(=N)N',                                           N'Antidiabetics',     N'First-line medication for type 2 diabetes mellitus.',                                                 N'https://upload.wikimedia.org/wikipedia/commons/thumb/d/d8/Metformin.svg/1200px-Metformin.svg.png'),
(N'Amaryl',       N'Glimepiride',               N'CCC1=C(C)CN(C(=O)NCCc2ccc(S(=O)(=O)NC(=O)NC3CCC(C)CC3)cc2)C1=O', N'Antidiabetics',  N'Sulfonylurea used for type 2 diabetes mellitus.',                                                     N'https://upload.wikimedia.org/wikipedia/commons/thumb/2/27/Glimepiride.svg/1200px-Glimepiride.svg.png'),
(N'Januvia',      N'Sitagliptin',               N'NC(CC(=O)N1CCn2c(nnc2C(F)(F)F)C1)Cc1cc(F)c(F)cc1F',           N'Antidiabetics',     N'DPP-4 inhibitor for type 2 diabetes.',                                                                N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/97/Sitagliptin.svg/1200px-Sitagliptin.svg.png'),
(N'Norvasc',      N'Amlodipine',                N'CCOC(=O)C1=C(COCCN)NC(C)=C(C(=O)OC)C1c1ccccc1Cl',             N'Antihypertensives', N'Calcium channel blocker for hypertension and angina.',                                                N'https://upload.wikimedia.org/wikipedia/commons/thumb/c/cf/Amlodipine.svg/1200px-Amlodipine.svg.png'),
(N'Cozaar',       N'Losartan',                  N'CCCCc1nc(Cl)c(CO)n1Cc1ccc(-c2ccccc2-c2nnn[nH]2)cc1',          N'Antihypertensives', N'Angiotensin II receptor blocker for hypertension.',                                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/5/5b/Losartan.svg/1200px-Losartan.svg.png'),
(N'Concor',       N'Bisoprolol',                N'CC(C)NCC(O)COc1ccc(COCCOC(C)C)cc1',                           N'Antihypertensives', N'Selective beta-1 blocker for hypertension and heart failure.',                                        N'https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Bisoprolol.svg/1200px-Bisoprolol.svg.png'),
(N'Capoten',      N'Captopril',                 N'CC(CS)C(=O)N1CCCC1C(=O)O',                                    N'Antihypertensives', N'ACE inhibitor for hypertension and heart failure.',                                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/2/29/Captopril.svg/1200px-Captopril.svg.png'),
(N'Lasix',        N'Furosemide',                N'O=C(O)c1cc(NCc2ccco2)c(S(N)(=O)=O)cc1Cl',                     N'Diuretics',         N'Loop diuretic for edema and hypertension.',                                                           N'https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Furosemide.svg/1200px-Furosemide.svg.png'),
(N'Lipitor',      N'Atorvastatin',              N'CC(C)c1c(C(=O)Nc2ccccc2)c(-c2ccccc2)c(-c2ccc(F)cc2)n1CCC(O)CC(O)CC(=O)O', N'Statins',N'HMG-CoA reductase inhibitor for cholesterol lowering.',                                                N'https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Atorvastatin.svg/1200px-Atorvastatin.svg.png'),
(N'Crestor',      N'Rosuvastatin',              N'CC(C)c1nc(N(C)S(C)(=O)=O)nc(-c2ccc(F)cc2)c1C=CC(O)CC(O)CC(=O)O', N'Statins',        N'Statin used to lower LDL cholesterol and triglycerides.',                                             N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/0d/Rosuvastatin.svg/1200px-Rosuvastatin.svg.png'),
(N'Zocor',        N'Simvastatin',               N'CCC(C)(C)C(=O)OC1CC(C)C=C2C=CC(C)C(CCC3CC(O)CC(=O)O3)C12',    N'Statins',           N'Statin medication used to control hypercholesterolemia.',                                             N'https://upload.wikimedia.org/wikipedia/commons/thumb/8/8e/Simvastatin.svg/1200px-Simvastatin.svg.png'),
(N'Plavix',       N'Clopidogrel',               N'COC(=O)C(c1ccccc1Cl)N1CCc2sccc2C1',                           N'Antiplatelets',     N'Antiplatelet agent for prevention of thrombotic events.',                                             N'https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Clopidogrel.svg/1200px-Clopidogrel.svg.png'),
(N'Xarelto',      N'Rivaroxaban',               N'O=C(NCC1CN(c2ccc(N3CCOCC3=O)cc2)C(=O)O1)c1ccc(Cl)s1',         N'Anticoagulants',    N'Direct factor Xa inhibitor for prevention of stroke and DVT.',                                        N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/9d/Rivaroxaban.svg/1200px-Rivaroxaban.svg.png'),
(N'Eliquis',      N'Apixaban',                  N'COc1ccc(-n2nc(C(N)=O)c3CCN(c4ccc(N5CCCCC5=O)cc4)C(=O)c23)cc1', N'Anticoagulants',   N'Oral anticoagulant for stroke prevention in atrial fibrillation.',                                    N'https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Apixaban.svg/1200px-Apixaban.svg.png'),
(N'Losec',        N'Omeprazole',                N'COc1ccc2[nH]c(S(=O)Cc3ncc(C)c(OC)c3C)nc2c1',                  N'PPI',               N'Proton pump inhibitor for GERD and peptic ulcers.',                                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Omeprazole.svg/1200px-Omeprazole.svg.png'),
(N'Nexium',       N'Esomeprazole',              N'COc1ccc2[nH]c(S(=O)Cc3ncc(C)c(OC)c3C)nc2c1',                  N'PPI',               N'Proton pump inhibitor for acid reflux and ulcers.',                                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/Esomeprazole.svg/1200px-Esomeprazole.svg.png'),
(N'Pantoloc',     N'Pantoprazole',              N'COc1ccc2[nH]c(S(=O)Cc3ncc(OC)cc3OC)nc2c1',                    N'PPI',               N'Proton pump inhibitor for gastric acid disorders.',                                                   N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/9b/Pantoprazole.svg/1200px-Pantoprazole.svg.png'),
(N'Motilium',     N'Domperidone',               N'O=C1Nc2ccccc2N1CCCN1CCC(N2C(=O)Nc3ccc(Cl)cc32)CC1',           N'Gastrointestinal',  N'Dopamine antagonist used as antiemetic and prokinetic.',                                              N'https://upload.wikimedia.org/wikipedia/commons/thumb/7/76/Domperidone.svg/1200px-Domperidone.svg.png'),
(N'Buscopan',     N'Hyoscine Butylbromide',     N'CCCC[N+]1(C)C2CCC1CC(OC(=O)C(CO)c1ccccc1)C2',                 N'Gastrointestinal',  N'Antispasmodic for abdominal cramps and IBS.',                                                         N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/96/Butylscopolamine.svg/1200px-Butylscopolamine.svg.png'),
(N'Ventolin',     N'Salbutamol',                N'CC(C)(C)NCC(O)c1ccc(O)c(CO)c1',                               N'Respiratory',       N'Short-acting beta-2 agonist bronchodilator for asthma and COPD.',                                     N'https://upload.wikimedia.org/wikipedia/commons/thumb/e/e1/Salbutamol.svg/1200px-Salbutamol.svg.png'),
(N'Symbicort',    N'Budesonide + Formoterol',   N'CCCC1OC2CC3C4CCC5=CC(=O)C=CC5(C)C4C(O)CC3(C)C2(O1)C(=O)CO',   N'Respiratory',       N'Combination inhaler for asthma and COPD maintenance.',                                                N'https://upload.wikimedia.org/wikipedia/commons/thumb/b/be/Budesonide.svg/1200px-Budesonide.svg.png'),
(N'Singulair',    N'Montelukast',               N'CC(C)(O)c1cccc(CCC(Sc2ccc3CC(c4cccc(C=Cc5ccc6ccc(Cl)cc6n5)c4)CC3c2)C(=O)O)c1', N'Respiratory', N'Leukotriene receptor antagonist for asthma and allergic rhinitis.',                                  N'https://upload.wikimedia.org/wikipedia/commons/thumb/2/2c/Montelukast.svg/1200px-Montelukast.svg.png'),
(N'Prozac',       N'Fluoxetine',                N'CNCCC(Oc1ccc(C(F)(F)F)cc1)c1ccccc1',                          N'Antidepressants',   N'SSRI antidepressant for depression and OCD.',                                                         N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/00/Fluoxetine.svg/1200px-Fluoxetine.svg.png'),
(N'Zoloft',       N'Sertraline',                N'CNC1CCC(c2ccc(Cl)c(Cl)c2)c2ccccc21',                          N'Antidepressants',   N'SSRI used to treat depression, panic disorder and PTSD.',                                             N'https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Sertraline.svg/1200px-Sertraline.svg.png'),
(N'Cipralex',     N'Escitalopram',              N'CN(C)CCCC1(c2ccc(C#N)cc2)OCc2cc(F)ccc21',                     N'Antidepressants',   N'SSRI for major depressive disorder and generalized anxiety.',                                          N'https://upload.wikimedia.org/wikipedia/commons/thumb/d/d9/Escitalopram.svg/1200px-Escitalopram.svg.png'),
(N'Xanax',        N'Alprazolam',                N'Cc1nnc2CN=C(c3ccccc3)c3cc(Cl)ccc3-n12',                       N'Antianxiety',       N'Benzodiazepine for anxiety and panic disorders.',                                                     N'https://upload.wikimedia.org/wikipedia/commons/thumb/9/91/Alprazolam.svg/1200px-Alprazolam.svg.png'),
(N'Tramal',       N'Tramadol',                  N'COc1cccc(C2(O)CCCCC2CN(C)C)c1',                               N'Opioid Analgesics', N'Centrally acting analgesic for moderate to severe pain.',                                             N'https://upload.wikimedia.org/wikipedia/commons/thumb/0/0a/Tramadol.svg/1200px-Tramadol.svg.png'),
(N'Cataract Drop',N'Latanoprost',               N'CCCCC(O)CCC1C(O)CC(O)C1CCC=CCCCC(=O)OC(C)C',                  N'Ophthalmic',        N'Prostaglandin analogue for glaucoma and ocular hypertension.',                                        N'https://upload.wikimedia.org/wikipedia/commons/thumb/c/cc/Latanoprost.svg/1200px-Latanoprost.svg.png');

DECLARE @Forms TABLE (
    FormIdx INT IDENTITY(1,1),
    FormName  NVARCHAR(100),
    DosageStr NVARCHAR(100)
);
INSERT INTO @Forms (FormName, DosageStr) VALUES
(N'Tablets 250mg',          N'1 tablet every 8 hours'),
(N'Tablets 500mg',          N'1 tablet every 8 hours'),
(N'Capsules 500mg',         N'1 capsule twice daily'),
(N'Syrup 120mg/5ml',        N'5 ml every 6 hours'),
(N'Suspension 250mg/5ml',   N'5 ml every 12 hours'),
(N'Effervescent 500mg',     N'1 tablet dissolved in water twice daily'),
(N'Extended Release 100mg', N'1 tablet once daily'),
(N'Injection 1g/Vial',      N'1 vial IV every 12 hours'),
(N'Drops 100mg/ml',         N'10 drops three times daily'),
(N'Sachets 1g',             N'1 sachet daily after meals');

;WITH Companies_CTE AS (
    SELECT Id, Name, ROW_NUMBER() OVER (ORDER BY Id) AS rn,
           COUNT(*) OVER () AS total
    FROM Companies
),
Cats_CTE AS (
    SELECT Id, Name FROM Categories
)
INSERT INTO Medicines
(Name, Description, Dosage, Price, ExpiryDate, ActiveIngredient, ImageUrl, Smiles, CompanyId, CategoryId, CreatedAt, UpdatedAt)
SELECT
    b.BaseName + N' ' + f.FormName,
    b.Description,
    f.DosageStr,
    CAST( (10 + ((b.Idx * 7 + f.FormIdx * 3) % 490) + 0.99) AS DECIMAL(18,2)),
    DATEADD(MONTH, 6 + ((b.Idx + f.FormIdx) % 30), GETDATE()),
    b.ActiveIngredient,
    b.ImageUrl,
    b.Smiles,
    (SELECT Id FROM Companies_CTE WHERE rn = (((b.Idx + f.FormIdx) % total) + 1)),
    c.Id,
    GETDATE(), GETDATE()
FROM @Bases b
CROSS JOIN @Forms f
INNER JOIN Cats_CTE c ON c.Name = b.CategoryName;
GO

;WITH 
Pharm AS (
    SELECT Id AS PharmId,
           ROW_NUMBER() OVER (ORDER BY Id) AS PRn
    FROM Pharmacies
    WHERE IsDeleted = 0
),
Med AS (
    SELECT Id          AS MedId,
           Price       AS BasePrice,
           ExpiryDate  AS MedExpiry,
           ROW_NUMBER() OVER (ORDER BY Id) AS MRn
    FROM Medicines
),
Joined AS (
    SELECT
        p.PharmId, m.MedId, m.BasePrice, m.MedExpiry, p.PRn, m.MRn,
        ((p.PRn * 13 + m.MRn * 7) % 100)  AS Seed1,
        ((p.PRn * 17 + m.MRn * 11) % 500) AS Seed2,
        ((p.PRn *  3 + m.MRn) % 21)       AS PriceSeed,
        ((p.PRn + m.MRn) % 30)            AS DateSeed
    FROM Pharm p
    CROSS JOIN Med m
)
INSERT INTO InventoryItems
(
import {
  Anchor,
  Box,
  Button,
  Grid,
  Group,
  Image,
  Paper,
  Stack,
  Text,
  Title,
  useMantineTheme,
} from '@mantine/core'
import { FC, useState } from 'react'
import { WithNavBar } from '@Components/WithNavbar'
import { useIsMobile } from '@Utils/ThemeOverride'
import { usePageTitle } from '@Hooks/usePageTitle'

type SectionKey = 'intro' | 'lecturers' | 'structure'

const sectionTitleMap: Record<SectionKey, string> = {
  intro: 'Giới thiệu chung',
  lecturers: 'Đội ngũ giảng viên',
  structure: 'Cơ cấu tổ chức',
}

const Home: FC = () => {
  const isMobile = useIsMobile(900)
  const [activeSection, setActiveSection] = useState<SectionKey>('intro')

  usePageTitle()

  return (
    <WithNavBar minWidth={0} withFooter withHeader stickyHeader>
      <Stack gap="xl">
        {/* HERO */}
        <Box
          style={{
            position: 'relative',
            height: isMobile ? 260 : 400,
            overflow: 'hidden',
          }}
        >
          <Box
            style={{
              position: 'absolute',
              inset: 0,
              backgroundImage:
                'url(https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-gioi-thieu-2-1536x657.jpg)',
              backgroundSize: 'cover',
              backgroundPosition: 'center',
              filter: 'brightness(0.55)',
              transform: 'scale(1.02)',
            }}
          />
          <Box
            style={{
              position: 'relative',
              zIndex: 1,
              maxWidth: 1200,
              margin: '0 auto',
              height: '100%',
              padding: '24px 16px',
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              textAlign: 'center',
            }}
          >
            <Title order={1} c="white" mt="xs">
              {sectionTitleMap[activeSection]}
            </Title>
            <Text mt={8} c="gray.2" size="sm" maw={480}>
              Không gian học tập, nghiên cứu và trải nghiệm cho sinh viên yêu
              thích an toàn thông tin &amp; CTF.
            </Text>
          </Box>
        </Box>

        {/* MAIN CONTENT + SIDE NAV */}
        <Box
          style={{
            maxWidth: 1200,
            margin: '0 auto',
            padding: '24px 16px 32px',
          }}
        >
          <Grid gutter={32}>
            <Grid.Col span={isMobile ? 12 : 8}>
              <MainContent activeSection={activeSection} />
            </Grid.Col>

            <Grid.Col span={isMobile ? 12 : 4}>
              <SideNav
                activeSection={activeSection}
                onChange={setActiveSection}
              />
            </Grid.Col>
          </Grid>
        </Box>
      </Stack>
    </WithNavBar>
  )
}

export default Home

/* --------- SIDE NAV + MAIN CONTENT ---------- */

interface MainContentProps {
  activeSection: SectionKey
}

const MainContent: FC<MainContentProps> = ({ activeSection }) => {
  return (
    <Stack gap="md">
      {activeSection === 'intro' && <IntroSection />}
      {activeSection === 'lecturers' && <LecturerSection />}
      {activeSection === 'structure' && <StructureSection />}
    </Stack>
  )
}

interface NavButtonProps {
  label: string
  active?: boolean
  onClick: () => void
}

const NavButton: FC<NavButtonProps> = ({ label, active, onClick }) => {
  const theme = useMantineTheme()
  return (
    <Button
      fullWidth
      variant={active ? 'filled' : 'outline'}
      color="blue"
      size="sm"
      radius="xs"
      onClick={onClick}
      styles={{
        root: {
          justifyContent: 'flex-start',
          fontWeight: active ? 600 : 500,
          backgroundColor: active ? theme.colors.blue[6] : theme.white,
        },
        label: {
          whiteSpace: 'normal', // cho phép xuống dòng
          lineHeight: 1.2,
          textAlign: 'center',
        },
      }}
    >
      {label}
    </Button>
  )
}


interface SideNavProps {
  activeSection: SectionKey
  onChange: (section: SectionKey) => void
}

const SideNav: FC<SideNavProps> = ({ activeSection, onChange }) => {
  return (
    <Paper withBorder radius="md" p="md" shadow="xs">
      <Text fw={600} mb="xs" size="sm" tt="uppercase">
        Trang
      </Text>
      <Stack gap={8}>
        <NavButton
          label="Giới thiệu chung"
          active={activeSection === 'intro'}
          onClick={() => onChange('intro')}
        />
        <NavButton
          label="Đội ngũ giảng viên"
          active={activeSection === 'lecturers'}
          onClick={() => onChange('lecturers')}
        />
        <NavButton
          label="Cơ cấu tổ chức"
          active={activeSection === 'structure'}
          onClick={() => onChange('structure')}
        />
      </Stack>
    </Paper>
  )
}

/* --------- SECTION: GIỚI THIỆU CHUNG ---------- */

const IntroSection: FC = () => (
  <Stack gap="md">
    <Title order={3} size="h3">
      Tổng quan
    </Title>
    <Text size="sm">
      Khoa An toàn thông tin là đơn vị đào tạo trực thuộc Học viện Công nghệ Bưu
      chính Viễn thông, có chức năng đào tạo và nghiên cứu khoa học trong lĩnh
      vực an toàn thông tin.
    </Text>

    <Title order={4} size="h4" mt="sm">
      Chức năng
    </Title>
    <Text size="sm">
      Khoa tổ chức hoạt động đào tạo đại học, sau đại học; nghiên cứu khoa học;
      hợp tác trong nước và quốc tế trong lĩnh vực an toàn, an ninh thông tin.
    </Text>

    <Title order={4} size="h4" mt="sm">
      Nhiệm vụ
    </Title>
    <Text component="ul" size="sm" style={{ paddingLeft: 18 }}>
      <li>Đào tạo nguồn nhân lực chất lượng cao về an toàn thông tin.</li>
      <li>
        Tổ chức nghiên cứu, chuyển giao công nghệ phục vụ chuyển đổi số và bảo
        đảm an toàn thông tin.
      </li>
      <li>Tăng cường hợp tác với doanh nghiệp, tổ chức trong và ngoài nước.</li>
    </Text>

    <Title order={4} size="h4" mt="sm">
      Các bộ môn thuộc Khoa
    </Title>
    <Text size="sm">
      Bộ môn An toàn mạng, Bộ môn An toàn phần mềm, cùng các nhóm nghiên cứu
      chuyên sâu về mật mã, forensics, phản ứng sự cố, CTF,...
    </Text>
  </Stack>
)

/* --------- SECTION: ĐỘI NGŨ GIẢNG VIÊN ---------- */

type Lecturer = {
  name: string
  position: string
  email: string
  avatar: string
  scholarUrl?: string
}

const LECTURERS: Lecturer[] = [
  {
    name: 'PGS. TS. Hoàng Xuân Dậu',
    position: 'Trưởng Khoa',
    email: 'dauhx@ptit.edu.vn',
    avatar:
      'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/z6295807069778_3eb18976ae2b9d4f53458b0ae9721705.jpg',
    scholarUrl: 'https://scholar.google.com.vn/citations?user=OUx-Gs4AAAAJ&hl=vi&oi=sra',
  },
  {
    name: 'TS. Nguyễn Ngọc Điệp',
    position: 'Trưởng Bộ môn An toàn mạng',
    email: 'diepnguyenngoc@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-the-3.jpg',
    scholarUrl: 'https://scholar.google.com.vn/citations?user=4YmlCM8AAAAJ&hl=vi',
  },
  {
    name: 'PGS. TS. Đỗ Xuân Chợ',
    position: 'Trưởng Bộ môn An toàn phần mềm',
    email: 'chodx@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Do-Xuan-Cho.jpg',
    scholarUrl: 'https://scholar.google.com.vn/citations?hl=vi&user=GHEAMuMAAAAJ',
  },
  {
    name: 'TS. Đinh Trường Duy',
    position: 'Giảng viên',
    email: 'duydt@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-chan-dung-2024.png',
    scholarUrl: 'https://scholar.google.com/citations?user=Pc42sHMAAAAJ&hl=vi&oi=ao',
  },
  {
    name: 'TS. Phạm Hoàng Duy',
    position: 'Giảng viên',
    email: 'duyph@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Anh-the.jpg',
  },
  {
    name: 'ThS. Ninh Thị Thu Trang',
    position: 'Giảng viên',
    email: 'trangntt2@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-TrangNTT.png',
    scholarUrl: 'https://scholar.google.com.vn/citations?hl=vi&user=1n6mj24AAAAJ',
  },
  {
    name: 'ThS. Nguyễn Hoa Cương',
    position: 'Giảng viên',
    email: 'cuongnh@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Ng-Hoa-Cuong-scaled.jpg',
  },
  {
    name: 'ThS. Vũ Minh Mạnh',
    position: 'Giảng viên',
    email: 'manhvm@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Vu-Minh-Manh.jpg',
    scholarUrl: 'https://scholar.google.be/citations?user=J9LoSGwAAAAJ&hl=en',
  },
  {
    name: 'TS. Quản Trọng Thế',
    position: 'Giảng viên',
    email: 'theqt@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-the-1.jpg',
  },
  {
    name: 'ThS. Phạm Trần Lan Anh',
    position: 'Trợ lý khoa',
    email: 'anhptl@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-the-2.jpg',
  },
  {
    name: 'PGS. TS. Trần Đức Sự',
    position: 'Giảng viên',
    email: 'sutd@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Anh-the-5.jpg',
    scholarUrl: 'https://scholar.google.com/citations?user=qd6rxzQAAAAJ&hl=en',
  },
  {
    name: 'TS. Nguyễn Trung Thành',
    position: 'Giảng viên',
    email: 'thanhnt3@ptit.edu.vn',
    avatar:
      'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Anh-Nguyen-Trung-Thanh.jpeg',
    scholarUrl: 'https://scholar.google.com/citations?user=qd6rxzQAAAAJ&hl=en',
  },
  {
    name: 'TS. Đặng Minh Tuấn',
    position: 'Giảng viên',
    email: 'tuandm@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/anh-the-4.jpg',
    scholarUrl: 'https://scholar.google.com/citations?user=qd6rxzQAAAAJ&hl=en',
  },
  {
    name: 'TS. Đặng Vũ Sơn',
    position: 'Giảng viên',
    email: 'dvson@ptit.edu.vn',
    avatar: 'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/Anh-2.jpg',
    scholarUrl: 'https://scholar.google.com/citations?user=qd6rxzQAAAAJ&hl=en',
  },
  {
    name: 'TS. Nguyễn Nam Hải',
    position: 'Giảng viên',
    email: 'hainn@ptit.edu.vn',
    avatar:
      'https://infosec.ptit.edu.vn/wp-content/uploads/sites/20/2025/02/z6302355354365_46739f2271f14f1afd60585abfe78aab.jpg',
  },
]

const LecturerSection: FC = () => {
  return (
    <Stack gap="sm">
      <Title order={3} size="h3" mb="sm">
        Đội ngũ giảng viên
      </Title>

      {LECTURERS.map((lec) => (
        <Paper
          key={lec.email}
          withBorder
          radius="md"
          p="sm"
          shadow="xs"
          style={{ display: 'flex' }}
        >
          <Group align="flex-start" gap="md" wrap="nowrap" style={{ width: '100%' }}>
            {/* ẢNH GIẢNG VIÊN – CHUẨN KÍCH THƯỚC */}
            <Box
              w={110}
              h={140}
              style={{
                flexShrink: 0,
                borderRadius: 8,
                overflow: 'hidden',
                backgroundColor: '#f3f4f6',
              }}
            >
              <Image
                src={lec.avatar}
                alt={lec.name}
                width="100%"
                height="100%"
                fit="cover"
                styles={{
                  image: {
                    objectPosition: 'center top', // hoặc 'center center'
                  },
                }}
              />
            </Box>

            <Stack gap={4} style={{ flex: 1 }}>
              <Text fw={600} size="sm">
                {lec.name}
              </Text>
              <Text size="xs" c="blue.7">
                {lec.position}
              </Text>
              <Text size="xs">
                <strong>Email: </strong>
                <Anchor size="xs" href={`mailto:${lec.email}`} underline="hover">
                  {lec.email}
                </Anchor>
              </Text>
              {lec.scholarUrl && (
                <Text size="xs">
                  <strong>Lý lịch khoa học: </strong>
                  <Anchor
                    size="xs"
                    href={lec.scholarUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    underline="hover"
                  >
                    Xem trên Google Scholar
                  </Anchor>
                </Text>
              )}
            </Stack>
          </Group>
        </Paper>
      ))}
    </Stack>
  )
}

/* --------- SECTION: CƠ CẤU TỔ CHỨC ---------- */

const StructureSection: FC = () => (
  <Stack gap="sm">
    <Title order={3} size="h3">
      Cơ cấu tổ chức
    </Title>
    <Text size="sm">
      Cơ cấu tổ chức của Khoa An toàn thông tin được xây dựng theo mô hình mở,
      thuận lợi cho việc phối hợp đào tạo, nghiên cứu và hợp tác với doanh
      nghiệp.
    </Text>
    <Text component="ul" size="sm" style={{ paddingLeft: 18 }}>
      <li>Ban chủ nhiệm Khoa.</li>
      <li>Bộ môn An toàn mạng.</li>
      <li>Bộ môn An toàn phần mềm.</li>
      <li>Các nhóm nghiên cứu &amp; phòng lab chuyên đề.</li>
    </Text>
  </Stack>
)

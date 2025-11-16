import {
  Anchor,
  Avatar,
  Badge,
  Box,
  Card,
  Container,
  Group,
  SimpleGrid,
  Stack,
  Text,
  Title,
  useMantineTheme,
} from '@mantine/core'
import { Icon } from '@mdi/react'
import {
  mdiFileDocumentOutline,
  mdiGithub,
  mdiScaleBalance,
  mdiTag,
  mdiAccountGroup,
  mdiLink,
} from '@mdi/js'
import { FC } from 'react'
import { useTranslation } from 'react-i18next'
import contributorsData from 'virtual:contributors'
import { WithNavBar } from '@Components/WithNavbar'
import { MainIcon } from '@Components/icon/MainIcon'
import { useIsMobile } from '@Utils/ThemeOverride'
import { ValidatedRepoMeta } from '@Hooks/useConfig'
import { usePageTitle } from '@Hooks/usePageTitle'
import classes from '@Styles/About.module.css'

const About: FC = () => {
  const { t } = useTranslation()
  const theme = useMantineTheme()
  const isMobile = useIsMobile()

  const { repo, valid, rawTag: tag, sha, buildTime } = ValidatedRepoMeta()
  const shortSha = sha ? `#${sha.substring(0, 8)}` : ''
  const buildText = valid
    ? `Built at ${buildTime.format('YYYY-MM-DDTHH:mm:ssZ')}`
    : 'This release is not officially built'

  usePageTitle(t('common.title.about'))

  return (
    <WithNavBar minWidth={0} withFooter withHeader stickyHeader>
      <Box py="xl" className={classes.container}>
        <Container size="lg">
          <Stack gap="xl">
            {/* HERO */}
            <HeroSection isMobile={isMobile} repo={repo} />

            {/* INTRO + MISSION / VISION / VALUES */}
            <IntroSection />

            {/* FEATURES */}
            <FeatureSection />

            {/* FOR WHO */}
            <AudienceSection />

            {/* CONTRIBUTORS */}
          </Stack>
        </Container>
      </Box>
    </WithNavBar>
  )
}

export default About

/* ========== SECTIONS ========== */

interface HeroProps {
  isMobile: boolean
  repo: string
}

const HeroSection: FC<HeroProps> = ({ isMobile }) => {
  return (
    <Card
      withBorder
      radius="xl"
      shadow="sm"
      p={isMobile ? 'lg' : 'xl'}
      style={{
        background:
          'linear-gradient(135deg, rgba(37,99,235,0.12), rgba(59,130,246,0.03))',
      }}
    >
      <Group
        justify="space-between"
        align="center"
        gap="lg"
        wrap={isMobile ? 'wrap' : 'nowrap'}
      >
        <Group align="center" gap="md">
          <img src="	http://localhost:62000/assets/566b436f1265548443c41fb4554ce52d6a04691d20170354f51f88b2e0f253da/logo" alt="" width={"50"} />
          <Stack gap={2}>
            <Title order={1} size={isMobile ? '1.8rem' : '2.2rem'}>
              KHOA AN TOÀN THÔNG TIN – PTIT
            </Title>
            <Text size="sm" c="dimmed">
              Nền tảng luyện tập & tổ chức CTF cho sinh viên yêu thích an toàn
              thông tin, hướng tới mô hình “học qua thực chiến”.
            </Text>
          </Stack>
        </Group>

        <Stack gap={4} align={isMobile ? 'flex-start' : 'flex-end'}>
          <Badge size="lg" variant="light" color="blue">
            &gt; hacking to learn
          </Badge>
          <Text size="xs" c="dimmed">
            CTF • Thực hành phòng thủ • Môi trường lab an toàn
          </Text>
        </Stack>
      </Group>
    </Card>
  )
}

const IntroSection: FC = () => {
  return (
    <Stack gap="md">
      <Title order={2} size="h3">
        Giới thiệu
      </Title>
      <Text size="sm">
        Nền tảng này được tùy biến cho Khoa An toàn thông tin – Học viện Công
        nghệ Bưu chính Viễn thông, tập trung vào việc tổ chức các cuộc thi CTF,
        bài lab thực hành và không gian học tập cộng đồng cho sinh viên.
      </Text>

      <SimpleGrid cols={{ base: 1, md: 3 }} spacing="md" mt="sm">
        <Card radius="md" withBorder>
          <Stack gap={6}>
            <Group gap="xs">
              <Icon path={mdiLink} size={0.9} />
              <Text fw={600} size="sm">
                Sứ mệnh
              </Text>
            </Group>
            <Text size="sm" c="dimmed">
              Tạo ra một môi trường học tập thực tế, nơi sinh viên có thể luyện
              tập kỹ năng tấn công/phòng thủ, tư duy phân tích và làm việc nhóm.
            </Text>
          </Stack>
        </Card>
        <Card radius="md" withBorder>
          <Stack gap={6}>
            <Group gap="xs">
              <Icon path={mdiAccountGroup} size={0.9} />
              <Text fw={600} size="sm">
                Tầm nhìn
              </Text>
            </Group>
            <Text size="sm" c="dimmed">
              Trở thành nền tảng CTF & lab nội bộ giúp sinh viên ATTT PTIT đủ
              năng lực tham gia các cuộc thi trong nước và quốc tế.
            </Text>
          </Stack>
        </Card>
        <Card radius="md" withBorder>
          <Stack gap={6}>
            <Group gap="xs">
              <Icon path={mdiTag} size={0.9} />
              <Text fw={600} size="sm">
                Giá trị cốt lõi
              </Text>
            </Group>
            <Text size="sm" c="dimmed">
              Thực chiến – Chia sẻ – Trung thực học thuật – Tôn trọng cộng
              đồng & văn hóa mã nguồn mở.
            </Text>
          </Stack>
        </Card>
      </SimpleGrid>
    </Stack>
  )
}

const FeatureSection: FC = () => {
  return (
    <Stack gap="md" mt="lg">
      <Title order={2} size="h3">
        Tính năng chính
      </Title>
      <SimpleGrid cols={{ base: 1, md: 3 }} spacing="md">
        <Card withBorder radius="md" shadow="xs">
          <Stack gap={6}>
            <Text fw={600} size="sm">
              Quản lý CTF & cuộc thi
            </Text>
            <Text size="sm" c="dimmed">
              Tạo game, chia đội, chấm điểm realtime, scoreboard rõ ràng – phù
              hợp cho cả CTF nội bộ và các sự kiện lớn.
            </Text>
          </Stack>
        </Card>
        <Card withBorder radius="md" shadow="xs">
          <Stack gap={6}>
            <Text fw={600} size="sm">
              Lab thực hành & bài tập
            </Text>
            <Text size="sm" c="dimmed">
              Hạ tầng container hóa cho từng bài lab, cô lập với hệ thống chính
              giúp sinh viên thử nghiệm thoải mái nhưng vẫn an toàn.
            </Text>
          </Stack>
        </Card>
        <Card withBorder radius="md" shadow="xs">
          <Stack gap={6}>
            <Text fw={600} size="sm">
              Tích hợp chấm điểm & báo cáo
            </Text>
            <Text size="sm" c="dimmed">
              Hỗ trợ nộp flag, báo cáo, tổng hợp điểm theo lớp/môn học; giảng
              viên dễ dàng theo dõi tiến độ sinh viên.
            </Text>
          </Stack>
        </Card>
      </SimpleGrid>
    </Stack>
  )
}

const AudienceSection: FC = () => {
  return (
    <Stack gap="md" mt="lg">
      <Title order={2} size="h3">
        Nền tảng dành cho ai?
      </Title>
      <SimpleGrid cols={{ base: 1, md: 3 }} spacing="md">
        <Card withBorder radius="md">
          <Stack gap={6}>
            <Text fw={600} size="sm">
              Sinh viên
            </Text>
            <Text size="sm" c="dimmed">
              Luyện CTF, làm lab từ cơ bản đến nâng cao, tham gia giải thật,
              xây CV & hồ sơ học thuật nổi bật.
            </Text>
          </Stack>
        </Card>
        <Card withBorder radius="md">
          <Stack gap={6}>
            <Text fw={600} size="sm">
              Giảng viên
            </Text>
            <Text size="sm" c="dimmed">
              Tổ chức buổi thực hành, thiết kế bài lab, theo dõi tiến độ và đánh
              giá năng lực sinh viên theo từng lớp/môn học.
            </Text>
          </Stack>
        </Card>
        <Card withBorder radius="md">
          <Stack gap={6}>
            <Text fw={600} size="sm">
              Cộng đồng ATTT
            </Text>
            <Text size="sm" c="dimmed">
              Là nơi chia sẻ writeup, ý tưởng bài, tổ chức sự kiện CTF mở, kết
              nối sinh viên với doanh nghiệp & chuyên gia.
            </Text>
          </Stack>
        </Card>
      </SimpleGrid>
    </Stack>
  )
}

/* --------- CONTRIBUTORS + VERSION ---------- */
